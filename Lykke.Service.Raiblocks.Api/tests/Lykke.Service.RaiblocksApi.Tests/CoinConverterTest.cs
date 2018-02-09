using System;
using Lykke.Service.RaiblocksApi.Core.Helpers;
using Xunit;

namespace Lykke.Service.RaiblocksApi.Tests
{
    public class CoinConverterTest
    {
        [Fact]
        public void LykkeRaiToRaw28_30Test()
        {
            var converter = new CoinConverter(28, 30);

            var raw = converter.LykkeRaiToRaw(1.ToString());

            Assert.Equal("100", raw);
        }

        [Fact]
        public void LykkeRaiToRaw6_30Test()
        {
            var converter = new CoinConverter(6, 30);

            var raw = converter.LykkeRaiToRaw(1.ToString());

            Assert.Equal("1000000000000000000000000", raw);
        }

        [Fact]
        public void ykkeRaiToRaw6_30ApproxTest()
        {
            var converter = new CoinConverter(6, 30);

            var raw = converter.LykkeRaiToRaw("11");

            Assert.Equal("11000000000000000000000000", raw);
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
        public void RawToLykkeRai28_30Test()
        {
            var converter = new CoinConverter(28, 30);

            var lykkeRai = converter.RawToLykkeRai(100.ToString());

            Assert.Equal("1", lykkeRai);
        }

        [Fact]
        public void RawToLykkeRai6_30Test()
        {
            var converter = new CoinConverter(6, 30);

            var lykkeRai = converter.RawToLykkeRai("1000000000000000000000000");

            Assert.Equal("1", lykkeRai);
        }

        [Fact]
        public void RawToLykkeRai6_30Approx1Test()
        {
            var converter = new CoinConverter(6, 30);

            var lykkeRai = converter.RawToLykkeRai("11111111111111111111111111");

            Assert.Equal("11", lykkeRai);
        }

        [Fact]
        public void RawToLykkeRai6_30Approx9Test()
        {
            var converter = new CoinConverter(6, 30);

            var lykkeRai = converter.RawToLykkeRai("11999999999999999999999999");

            Assert.Equal("11", lykkeRai);
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
