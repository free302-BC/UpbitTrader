using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Calc;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic
{
    public interface IBackTest
    {
        static (int trades, decimal rate, decimal mdd) 
            BackTest(CandleModel[] models,  int offset, int count, ICalcParam param)
        {
            if (param.WindowFunction != WindowFunction.None) 
                ICalcCandle.CalcMovingAvg(models, offset, count, param);

            ICalcCandle.CalcProfitRate(models, offset, count, param);
            var trades = models.Take(count).Count(x => x.BtSignal == BackTestSignal.DoTrade);

            var rate = ICalcCandle.CalcCumRate(models, offset, count);
            var mdd = ICalcCandle.CalcDrawDown(models, offset, count);
            return (trades, rate, mdd);
        }
    }
}
