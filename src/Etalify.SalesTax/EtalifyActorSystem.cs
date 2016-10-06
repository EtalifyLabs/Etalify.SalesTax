using System;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Ninject;
using Akka.Routing;
using Etalify.SalesTax.Actors;
using Etalify.SalesTax.Configuration;
using Ninject;
using Ninject.Modules;

namespace Etalify.SalesTax
{
    public class EtalifyActorSystem : IActorSystem
    {
        private static readonly StandardKernel Container = new StandardKernel();

        /// <summary>
        ///     Gets the actor system. Can be null if the actor system has not yet been started.
        /// </summary>
        /// <value>
        ///     The system.
        /// </value>
        public ActorSystem System { get; private set; }

        /// <summary>
        ///     Gets a reference to the API actor.
        /// </summary>
        /// <value>
        ///     The API actor.
        /// </value>
        public IActorRef ApiActor { get; private set; }

        public EtalifyActorSystem(Configuration.Settings settings)
        {
            LoadModules(settings);
        }

        private void SetupSystem(ActorSystem system)
        {
            var resolver = new NinjectDependencyResolver(Container, system);

            SetupActors(system, resolver);
        }

        private void SetupActors(IActorRefFactory system, IDependencyResolver resolver)
        {
            var workers = new[] { "/user/coordinator/*" };

            var timeoutAfterNoRepliesWithin = TimeSpan.FromSeconds(30);
            var intervals = TimeSpan.FromMilliseconds(20);

            ApiActor = system.ActorOf(Props.Empty.WithRouter(new TailChoppingGroup(workers, timeoutAfterNoRepliesWithin, intervals)), "api");

            var coordinatorProps = resolver.Create<CoordinatorActor>();
            system.ActorOf(coordinatorProps, CoordinatorActor.Name);
        }

        private static void LoadModules(Configuration.Settings settings)
        {
            Container.Load(new NinjectModule[]
            {
                new SettingsModule(settings)
            });
        }

        /// <summary>
        ///     Starts this actor system and any required actors.
        /// </summary>
        public void Start()
        {
            System = ActorSystem.Create("EtalifySystem");

            SetupSystem(System);
        }

        /// <summary>
        ///     Stops this actor system and disposes of resources.
        /// </summary>
        public void Stop()
        {
            if (System == null)
            {
                return;
            }

            System.Terminate();
            System.Dispose();

            System = null;
        }
    }
}