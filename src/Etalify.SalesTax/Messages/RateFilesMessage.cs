using System;

namespace Etalify.SalesTax.Messages
{
    public class RateFilesMessage : IEquatable<RateFilesMessage>
    {
        public RateFilesMessage(string addressFile, string zipFile, string ratesFile)
        {
            AddressFile = addressFile;
            ZipFile = zipFile;
            RatesFile = ratesFile;
        }

        public string AddressFile { get; }

        public string ZipFile { get; }

        public string RatesFile { get; }

        public bool Equals(RateFilesMessage other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var addressEqual = string.Equals(AddressFile, other.AddressFile, StringComparison.OrdinalIgnoreCase);

            var zipEqual = string.Equals(ZipFile, other.ZipFile, StringComparison.OrdinalIgnoreCase);

            var ratesEqual = string.Equals(RatesFile, other.RatesFile, StringComparison.OrdinalIgnoreCase);

            return addressEqual && zipEqual && ratesEqual;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RateFilesMessage))
            {
                return false;
            }

            return Equals((RateFilesMessage) obj);
        }

        public override int GetHashCode()
        {
            const int prime = 397;

            var result = (AddressFile?.GetHashCode() ?? 0);

            result = (result*prime) ^ (ZipFile?.GetHashCode() ?? 0);
            result = (result * prime) ^ (RatesFile?.GetHashCode() ?? 0);

            return result;
        }
    }
}