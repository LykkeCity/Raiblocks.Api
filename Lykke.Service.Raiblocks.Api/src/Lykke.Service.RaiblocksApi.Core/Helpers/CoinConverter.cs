using System;
using System.Numerics;

namespace Lykke.Service.RaiblocksApi.Core.Helpers
{
    public class CoinConverter
    {
        private readonly int Accuracy;

        private readonly int MaxAccuracy;

        public CoinConverter(int accuracy, int maxAccuracy)
        {
            Accuracy = accuracy;
            MaxAccuracy = maxAccuracy;
        }

        public string LykkeRaiToRaw(string lykkeRai)
        {
            var result = BigInteger.TryParse(lykkeRai, out var lykkeRaiParsed);

            if (result)
            {
                return (lykkeRaiParsed * BigInteger.Pow(10, MaxAccuracy - Accuracy)).ToString();
            } else
            {
                throw new ArgumentException("Invalid lykkeRai value, must be BigInteger");
            }

        }

        public string RawToLykkeRai(string raw)
        {
            var result = BigInteger.TryParse(raw, out var rawParsed);

            if (result)
            {
                return (rawParsed / BigInteger.Pow(10, MaxAccuracy - Accuracy)).ToString();
            }
            else
            {
                throw new ArgumentException("Invalid raw value, must be BigInteger");
            }

        }
    }
}
