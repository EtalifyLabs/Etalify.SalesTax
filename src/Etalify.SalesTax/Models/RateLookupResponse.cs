namespace Etalify.SalesTax.Models
{
    public class RateLookupResponse : Messages.RateLookupResponse
    {
        public RateLookupResponse(string locationCode, string locationName, decimal totalRate) : base(locationCode, locationName, totalRate)
        {
        }
    }
}