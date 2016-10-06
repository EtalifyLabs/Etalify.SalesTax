using Akka.Actor;

namespace Etalify.SalesTax
{
    public interface IActorSystem
    {
        /// <summary>
        ///     Gets the actor system. Can be null if the actor system has not yet been started.
        /// </summary>
        /// <value>
        ///     The system.
        /// </value>
        ActorSystem System { get; }

        /// <summary>
        ///     Starts this actor system and any required actors.
        /// </summary>
        void Start();

        /// <summary>
        ///     Stops this actor system and disposes of resources.
        /// </summary>
        void Stop();

        /// <summary>
        ///     Gets a reference to the API actor.
        /// </summary>
        /// <value>
        ///     The API actor.
        /// </value>
        IActorRef ApiActor { get; }
    }
}
