using ServiceStack.Configuration;

namespace Etalify.SalesTax.Configuration
{
    public class Settings
    {
        public Settings()
        {
        }

        public Settings(IAppSettings appSettings)
        {
            RateFilesPath = appSettings.Get("RateFilesPath", @"c:\tmp");

            HostName = appSettings.Get("Hostname", "http://*:1337/");

            TaxServiceUrl = appSettings.GetString("TaxService.Url");

            TaxServiceApiKey = appSettings.GetString("TaxService.ApiKey");

            LocalFilesRefreshInMinutes = appSettings.Get("LocalFilesRefreshInMinutes", 60);
        }

        public string RateFilesPath { get; set; }

        public string HostName { get; set; }

        public string TaxServiceUrl { get; set; }

        public string TaxServiceApiKey { get; set; }

        public int LocalFilesRefreshInMinutes { get; set; }
    }
}