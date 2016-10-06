using Etalify.SalesTax.Configuration;
using Etalify.SalesTax.ServiceInterface;
using Funq;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Text;

namespace Etalify.SalesTax
{
    public class AppHost : AppSelfHostBase
    {
        private IActorSystem _actorSystem;

        /// <summary>
        ///     Gets the application settings instance.
        /// </summary>
        /// <value>
        ///     The settings.
        /// </value>
        public Settings Settings => Container?.Resolve<Settings>();

        public AppHost()
          : base("Etalify Sales Tax Service", typeof(RateLookupService).Assembly)
        { }

        public override void Configure(Container container)
        {
            Container.Register<IAppSettings>(c => new AppSettings());
            Container.Register(c => new Settings(c.Resolve<IAppSettings>()));

            Container.Register<IActorSystem>(c => new EtalifyActorSystem(c.Resolve<Settings>()));

            JsConfig.DateHandler = DateHandler.ISO8601;
            JsConfig.EmitCamelCaseNames = true;
            JsConfig.ExcludeDefaultValues = true;
        }

        public override ServiceStackHost Start(string urlBase)
        {
            _actorSystem = Container.Resolve<IActorSystem>();
            _actorSystem.Start();

            return base.Start(urlBase);
        }

        public override void Stop()
        {
            if (_actorSystem != null)
            {
                _actorSystem.Stop();
                _actorSystem = null;
            }

            base.Stop();
        }
    }
}
