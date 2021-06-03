using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Analysis;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic.Calc
{
    public class SimplePR : ICalcPR
    {
        public void CalcProfitRate(CandleModel model, CandleModel prev, decimal k)
        {
            model.Target = Math.Round(model.Opening + prev.Delta * k, 2);
            model.Rate = (model.High > model.Target)
                ? Math.Round(model.Closing / model.Target - CandleModel.FeeRate, 4)
                : 1.0000m;
        }
    }//class

    public class MovingAvgPR : ICalcPR
    {
        public void CalcProfitRate(CandleModel model, CandleModel prev, decimal k)
        {
            model.Target = Math.Round(model.Opening + prev.Delta * k, 2);
            model.Rate = (model.High > model.Target && model.Opening >= prev.MovingAvg)
                ? Math.Round(model.Closing / model.Target - CandleModel.FeeRate, 4)
                : 1.0m;
        }
    }//class
}
