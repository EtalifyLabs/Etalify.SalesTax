using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Etalify.SalesTax.Messages;
using Etalify.SalesTax.Models;
using ServiceStack;
using WaRateFiles;
using RateLookupRequest = Etalify.SalesTax.Messages.RateLookupRequest;
using RateLookupResponse = Etalify.SalesTax.Messages.RateLookupResponse;
using Settings = Etalify.SalesTax.Configuration.Settings;

namespace Etalify.SalesTax.Actors
{
    public class RateLookupActor : ReceiveActor, IWithUnboundedStash
    {
        private readonly RateFilesMessage _rateFiles;
        private readonly IActorRef _coordinatorActor;

        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private RateLookup _lookupService;
        private readonly Settings _settings;

        public IStash Stash { get; set; }

        public RateLookupActor(IActorRef coordinatorActor, RateFilesMessage rateFiles, Settings settings)
        {
            _rateFiles = rateFiles;
            _coordinatorActor = coordinatorActor;
            _settings = settings;

            WaitingForFiles();
        }

        protected override void PreStart()
        {
            _lookupService = new RateLookup
            (
                _rateFiles.AddressFile,
                _rateFiles.ZipFile,
                _rateFiles.RatesFile,
                RateLookupEngine.STANDARDIZER,
                false
            );

            BecomeReady();

            _coordinatorActor.Tell(_rateFiles);
        }

        protected override void PreRestart(Exception reason, object message)
        {
            //save whatever is in the stash for when we restart
            Stash.UnstashAll();
            base.PreRestart(reason, message);
        }

        protected override void PostStop()
        {
            _lookupService = null;

            /*
             * The GC really doesn't want to let go of the dictionaries in the _lookupService
             * but as they're so large, we force it.
             */
            GC.Collect();
        }

        private void WaitingForFiles()
        {
            // stash all messages until we've loaded the rate files
            ReceiveAny(o => Stash.Stash());
        }

        private void BecomeReady()
        {
            _logger.Info("RateLookupActor ready");

            Become(Ready);
            Stash.UnstashAll();
        }

        private void Ready()
        {
            Receive<RateLookupRequest>(x => string.IsNullOrEmpty(x.State) == false
                                            && x.State.Equals("WA", StringComparison.OrdinalIgnoreCase), LookupLocal);

            Receive<RateLookupRequest>(x => LookupRemote(x));
        }

        private void LookupLocal(RateLookupRequest lookup)
        {
            Rate rate = null;

            try
            {
                LocationSource locationSource;
                AddressLine addressLine;

                _lookupService.FindRate(lookup.Street, lookup.City, lookup.Zip, out addressLine, ref rate, out locationSource);

                var response = new RateLookupResponse(rate.LocationCode, rate.LocationName, rate.TotalRate);

                Sender.Tell(response, Self);
            }
            catch (Exception e)
            {
                Sender.Tell(new Failure { Exception = e }, Self);
            }
        }

        private void LookupRemote(RateLookupRequest lookup)
        {
            var sender = Sender;
            var self = Self;

            var url = _settings.TaxServiceUrl;

            if (string.IsNullOrEmpty(lookup.Zip) == false)
            {
                url = url + lookup.Zip;
            }

            if (string.IsNullOrEmpty(lookup.City) == false)
            {
                url = url.AddQueryParam("city", lookup.City);
            }

            url.GetStringFromUrlAsync(requestFilter: webReq => {
                webReq.Headers["Authorization"] = _settings.TaxServiceApiKey;
            }).ContinueWith(t =>
            {
                /*
                 * If the request completed OK with pipe the result back to the original sender,
                 * which is likely to be a web API request handler.
                 */
                if (t.IsFaulted || t.IsCanceled)
                {
                    sender.Tell(new Failure { Exception = t.Exception }, self);
                    return;
                }

                var result = t.Result.FromJson<TaxJarResponse>();

                if (result?.Result == null)
                {
                    Task.FromResult(null as RateLookupResponse).PipeTo(sender);
                    return;
                }

                var salesTaxResult = result.Result;

                if (salesTaxResult == null)
                {
                    Task.FromResult(null as RateLookupResponse).PipeTo(sender);
                    return;
                }

                var rate = Convert.ToDecimal(salesTaxResult.TotalRate);

                var response = new RateLookupResponse(salesTaxResult.Zip, salesTaxResult.City, rate);

                sender.Tell(response, self);

            }, TaskContinuationOptions.AttachedToParent);
        }
    }
}
