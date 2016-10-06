using System;

namespace Etalify.SalesTax.Models
{
    public class PeriodResponse
    {
        public int Number { get; set; }
        public int Year { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}