using Ninject.Modules;

namespace Etalify.SalesTax.Configuration
{
    public class SettingsModule : NinjectModule
    {
        private readonly Settings _settings;

        public SettingsModule(Settings settings)
        {
            _settings = settings;
        }

        public override void Load()
        {
            Bind<Settings>().ToMethod(c => _settings);
        }
    }
}