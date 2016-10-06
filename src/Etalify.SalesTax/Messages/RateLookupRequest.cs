namespace Etalify.SalesTax.Messages
{
    public class RateLookupRequest
    {
        public RateLookupRequest(string street, string city, string state, string zip)
        {
            Street = street;
            City = city;
            Zip = zip;
            State = state;
        }

        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }
    }
}