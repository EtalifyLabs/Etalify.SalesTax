using ServiceStack;

namespace Etalify.SalesTax.Models
{
    [Route("/rates")]
    public class RateLookupRequest : Messages.RateLookupRequest
    {
        public RateLookupRequest(string street, string city, string state, string zip) : base(street, city, state, zip)
        {
        }
    }
}