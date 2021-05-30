using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic
{
    public interface IBackTest
    {
        static (int trades, decimal rate, decimal mdd) 
            backTest(CandleModel[] models, decimal k, int offset, int count, bool applyMovingAvg)
        {
            if (!applyMovingAvg) SimplePR.Default.CalcProfitRate(models, k, offset, count);
            else MovingAvgPR.Default.CalcProfitRate(models, k, offset, count);

            var trades = models.Take(count).Count(x => x.Rate != 1m);

            var rate = IModelCalc.CalcCumRate(models, offset, count);
            var mdd = IModelCalc.CalcDrawDown(models, offset, count);
            return (trades, rate, mdd);
        }
    }
}
