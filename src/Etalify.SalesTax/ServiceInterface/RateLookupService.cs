using System.Threading.Tasks;
using Akka.Actor;
using Etalify.SalesTax.Models;
using ServiceStack;
using WaRateFiles;
using RateLookupResponse = Etalify.SalesTax.Messages.RateLookupResponse;

namespace Etalify.SalesTax.ServiceInterface
{
    public class RateLookupService : IService
    {
        private readonly IActorSystem _actorSystem;

        public RateLookupService(IActorSystem actorSystem)
        {
            _actorSystem = actorSystem;
        }

        public async Task<object> Any(RateLookupRequest request)
        {
            if (_actorSystem.ApiActor.Equals(ActorRefs.Nobody))
            {
                throw HttpError.Forbidden("Actor system is not ready");
            }

            var message = request.ConvertTo<Messages.RateLookupRequest>();

            var result = await _actorSystem.ApiActor.Ask(message) as RateLookupResponse;

            if (result == null)
            {
                throw HttpError.NotFound("The provided address was not found");
            }

            return new Models.RateLookupResponse
            (
                result.LocationCode,
                result.LocationName,
                result.TotalRate
            );
        }

        public object Get(PeriodRequest request)
        {
            var period = Period.CurrentPeriod();

            return new PeriodResponse
            {
                Number = period.PeriodNum,
                Year = period.Year,
                StartDate = period.StartDate.AsDateTime,
                EndDate = period.EndDate.AsDateTime
            };
        }
    }
}