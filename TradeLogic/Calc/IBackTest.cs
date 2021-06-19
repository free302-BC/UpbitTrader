using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic.Calc
{
    public interface IBackTest
    {
        static (int trades, decimal rate, decimal mdd) 
            BackTest(CandleModel[] models, ICalcParam param)
        {
            if (param.WindowFunction != WindowFunction.None) 
                ICalcCandle.CalcMovingAvg(models, param);

            ICalcCandle.CalcProfitRate(models, param);
            var trades = models.Count(x => x.TradeDone);

            var rate = ICalcCandle.CalcCumRate(models);
            var mdd = ICalcCandle.CalcDrawDown(models);
            return (trades, rate, mdd);
        }
    }
}
