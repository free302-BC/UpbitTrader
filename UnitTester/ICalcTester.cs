using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Calc;
using Xunit;

namespace UnitTester
{
    public class ICalcTester
    {
        [Fact] void MovingAvgTest() => ICalc.MovingAvgTest();
        [Fact] void MacdTest() => ICalc.MacdTest();
        [Fact] void NormalizedTest() => ICalc.NormalizedTest();
        [Fact] void MacdProfitRate() => ICalc.MacdProfitRate();

    }
}
