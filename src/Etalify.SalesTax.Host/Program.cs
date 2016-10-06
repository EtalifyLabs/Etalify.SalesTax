using System;
using Topshelf;

namespace Etalify.SalesTax.Host
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var exitCode = (int)HostFactory.Run(x =>
            {
                x.Service<AppHost>(s =>
                {
                    s.ConstructUsing(host =>
                    {
                        var service = new AppHost();

                        service.Init();

                        return service;
                    });

                    s.WhenStarted(service => service.Start(service.Settings.HostName));
                    s.WhenStopped(service => service.Stop());
                });

                x.SetServiceName("EtalifySalesTax");
                x.SetDisplayName("Etalify Sales Tax Service");
                x.SetDescription("Etalify Sales Tax scheduled jobs processing.");

                x.StartAutomatically();
                x.RunAsLocalSystem();

                x.EnableServiceRecovery(r => r.RestartService(1));
            });

            Environment.Exit(exitCode);
        }
    }
}