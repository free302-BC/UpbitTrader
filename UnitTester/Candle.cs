using System;
using System.Linq;
using Xunit;
using Universe.Coin.Upbit.Model;
using Xunit.Abstractions;
using Universe.Coin.Upbit;

namespace UnitTester
{
    public class Candle
    {
        private readonly ITestOutputHelper _out;
        public Candle(ITestOutputHelper output) => _out = output;
        
        [Fact]
        void Ctor_Throws_ArgumentException_Call_With_2()
        {
            Assert.Throws<ArgumentException>(() => new CandleMinute((CandleUnit)2));            
        }
        [Fact]
        void CandleUnit_Set_2_Throws_ArgumentException()
        {
            var candle = new CandleMinute();
            var unit = (CandleUnit)2;
            Assert.Throws<ArgumentException>(() => candle.CandleUnit = unit);
        }

    }
}
