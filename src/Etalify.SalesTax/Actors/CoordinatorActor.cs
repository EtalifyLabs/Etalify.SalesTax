using System;
using System.IO;
using Akka.Actor;
using Akka.Event;
using Etalify.SalesTax.Messages;
using WaRateFiles;
using Settings = Etalify.SalesTax.Configuration.Settings;

namespace Etalify.SalesTax.Actors
{
    public class CoordinatorActor : ReceiveActor
    {
        public const string Name = "coordinator";

        public class CheckMessage { }

        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private readonly Settings _settings;

        private RateFilesMessage _activeRates;
        private IActorRef _activeRateLookupActor;
        private ICancelable _recurringCheckTask;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CoordinatorActor"/> class and used by the actor system DI framework.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public CoordinatorActor(Settings settings)
        {
            _settings = settings;
        }

        protected override void PreStart()
        {
            Become(Ready);

            // schedule regular updates
            var refreshInMinutes = _settings.LocalFilesRefreshInMinutes;

            _recurringCheckTask = new Cancelable(Context.System.Scheduler);
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.Zero, TimeSpan.FromMinutes(refreshInMinutes), Self, new CheckMessage(), Self, _recurringCheckTask);
        }

        protected override void PreRestart(Exception reason, object message)
        {
            try
            {
                _recurringCheckTask.Cancel();
            }
            catch { }
        }

        private void Ready()
        {
            Receive<RateFilesMessage>(x => RateLookupsActive(x));

            Receive<CheckMessage>(x => Check());
        }

        private void Check()
        {
            CheckForUpdates();
            CheckForPeriodChange();
        }

        private void CheckForUpdates()
        {
            _logger.Info("Checking for updates");
            
            // This checks current period and next period
            if (FileMaintenance.IsUpdateAvailable(_settings.RateFilesPath))
            {
                FileMaintenance.UpdateFiles(_settings.RateFilesPath);
            }
        }

        private void CheckForPeriodChange()
        {
            _logger.Info("Checking for period changes");

            string addressFile, zipFile, rateFile;

            FileMaintenance.GetLocalFileBaseNames(Period.CurrentPeriod(), out addressFile, out zipFile, out rateFile);

            var rateFiles = new RateFilesMessage
            (
                Path.Combine(_settings.RateFilesPath, addressFile + ".csv"),
                Path.Combine(_settings.RateFilesPath, zipFile + ".csv"),
                Path.Combine(_settings.RateFilesPath, rateFile + ".csv")
            );

            if (_activeRateLookupActor == null || _activeRates == null || rateFiles.Equals(_activeRates) == false)
            {
                _logger.Info("Starting new actor");
                Context.ActorOf(Props.Create(() => new RateLookupActor(Self, rateFiles, _settings)));
            }
        }

        private void RateLookupsActive(RateFilesMessage rateFiles)
        {
            if (_activeRateLookupActor != null && _activeRateLookupActor.Equals(Sender) == false)
            {
                // kill old actor first
                _logger.Info("Killing old actor");
                _activeRateLookupActor.Tell(PoisonPill.Instance, Self);
            }

            _activeRateLookupActor = Sender;
            _activeRates = rateFiles;
        }
    }
}
