namespace Etalify.SalesTax.Messages
{
    public class RateLookupResponse
    {
        public RateLookupResponse(string locationCode, string locationName, decimal totalRate)
        {
            LocationCode = locationCode;
            LocationName = locationName;
            TotalRate = totalRate;
        }

        public string LocationCode { get; private set; }

        public string LocationName { get; private set; }

        public decimal TotalRate { get; private set; }
    }
}