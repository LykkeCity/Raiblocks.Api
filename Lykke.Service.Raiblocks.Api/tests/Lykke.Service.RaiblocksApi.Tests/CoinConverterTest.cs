using System;
using Lykke.Service.RaiblocksApi.Core.Helpers;
using Xunit;

namespace Lykke.Service.RaiblocksApi.Tests
{
    public class CoinConverterTest
    {
        [Fact]
        public void LykkeRaiToRawTest()
        {
            var converter = new CoinConverter(28, 30);

            var raw = converter.LykkeRaiToRaw(1.ToString());

            Assert.Equal("100", raw);
        }
        
        [Fact]
        public void LykkeRaiToRawErrorTest()
        {
            var converter = new CoinConverter(28, 30);

            Assert.Throws<ArgumentException>(() => converter.LykkeRaiToRaw("someString"));
            Assert.Throws<ArgumentException>(() => converter.LykkeRaiToRaw("2.1"));
            Assert.Throws<ArgumentException>(() => converter.LykkeRaiToRaw("2,1"));
        }
        
        [Fact]
        public void RawToLykkeRaiTest()
        {
            var converter = new CoinConverter(28, 30);

            var raw = converter.RawToLykkeRai(100.ToString());

            Assert.Equal("1", raw);
        }
        
        [Fact]
        public void RawToLykkeRaiErrorTest()
        {
            var converter = new CoinConverter(28, 30);

            Assert.Throws<ArgumentException>(() => converter.RawToLykkeRai("someString"));
            Assert.Throws<ArgumentException>(() => converter.RawToLykkeRai("2.1"));
            Assert.Throws<ArgumentException>(() => converter.RawToLykkeRai("2,1"));
        }
    }
}
