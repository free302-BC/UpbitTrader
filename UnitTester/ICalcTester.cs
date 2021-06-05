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
        [Fact]
        void test()
        {
            ICalc.MovingAvgTest();
            ICalc.MacdTest();
            ICalc.NormalizedTest();
        }

    }
}
