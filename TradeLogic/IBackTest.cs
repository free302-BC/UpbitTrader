using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic
{
    public interface IBackTest : ICalculator
    {
        static (int trades, decimal rate, decimal mdd) 
            BackTest(CandleModel[] models,  int offset, int count, ICalcParam param)
        {
            var mc = new ModelCalc(param);
            if (param.WindowFunction != WindowFunction.None) mc.CalcMovingAvg(models, offset, count);

            IProfitRate pr = param.WindowFunction != WindowFunction.None ? new MovingAvgPR(param) : new SimplePR(param);
            pr.CalcProfitRate(models, offset, count);
            var trades = models.Take(count).Count(x => x.Rate != 1m && x.Rate != 0m);

            var rate = mc.CalcCumRate(models, offset, count);
            var mdd = mc.CalcDrawDown(models, offset, count);
            return (trades, rate, mdd);
        }
    }
}
