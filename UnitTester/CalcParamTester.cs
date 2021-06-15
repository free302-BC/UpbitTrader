using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.App;
using Xunit;

namespace UnitTester
{
    public class CalcParamTester
    {
        [Fact]
        void test()
        {
            var p1 = new BackTestOptions();
            p1.CalcParam.FactorK = 0.123m;
            p1.CalcParam.MacdParam = new(1, 2, 3);
            var p2 = (BackTestOptions)p1.Clone();
            Assert.Equal(p1.CalcParam.FactorK, p2.CalcParam.FactorK);
            Assert.Equal(p1.CalcParam.MacdParam, p2.CalcParam.MacdParam);

            p1.CalcParam.FactorK = 3.14m;
            p1.CalcParam.MacdParam = new(4, 5, 6);
            p2.Reload(p1);
            Assert.Equal(p1.CalcParam.FactorK, p2.CalcParam.FactorK);
            Assert.Equal(p1.CalcParam.MacdParam, p2.CalcParam.MacdParam);

            Assert.Equal(p1.CalcParam.MacdParam, p2.CalcParam.MacdParam);
            var a = p1.CalcParam.MacdParam.Equals(p2.CalcParam.MacdParam);
            Assert.True(a);
        }


    }//class
}
