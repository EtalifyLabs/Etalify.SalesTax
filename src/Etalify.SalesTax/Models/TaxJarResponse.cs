using System.Runtime.Serialization;

namespace Etalify.SalesTax.Models
{
    [DataContract]
    public class TaxJarResponse
    {
        [DataMember(Name = "rate")]
        public RateResult Result { get; set; }

        [DataContract]
        public class RateResult
        {
            [DataMember(Name = "zip")]
            public string Zip { get; set; }

            [DataMember(Name = "city")]
            public string City { get; set; }

            [DataMember(Name = "county")]
            public string County { get; set; }

            [DataMember(Name = "state")]
            public string State { get; set; }

            [DataMember(Name = "combined_rate")]
            public string TotalRate { get; set; }

            [DataMember(Name = "city_rate")]
            public string CityRate { get; set; }
        }
    }
}