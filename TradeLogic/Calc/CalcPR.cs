using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Analysis;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic.Calc
{
    public class SimplePR : CalcBase, ICalcPR
    {
        public SimplePR(ICalcParam param) : base(param)
        {
        }
        public void CalcProfitRate(CandleModel[] models)
            => CalcProfitRate(models, 0, models.Length);

        public void CalcProfitRate(CandleModel[] models, int offset, int count)
        {
            calcProfitRate(models[offset], offset > 0 
                ? models[offset - 1] 
                : CandleModel.Empty, _param.FactorK);

            for (int i = offset + 1; i < offset + count && i < models.Length; i++)
                calcProfitRate(models[i], models[i - 1], _param.FactorK);
        }

        void calcProfitRate(CandleModel model, CandleModel prev, decimal k)
        {
            model.Target = Math.Round(model.Opening + prev.Delta * k, 2);
            model.Rate = (model.High > model.Target) 
                ? Math.Round(model.Closing / model.Target - CandleModel.FeeRate, 4) 
                : 1.0000m;
        }

    }//class

    public class MovingAvgPR :  CalcBase, ICalcPR
    {
        public MovingAvgPR(ICalcParam param) : base(param)
        {
        }
        public void CalcProfitRate(CandleModel[] models) 
            => CalcProfitRate(models, 0, models.Length);

        public void CalcProfitRate(CandleModel[] models, int offset, int count)
        {
            calcProfitRate(models[offset], offset > 0 
                ? models[offset - 1] 
                : CandleModel.Empty, _param.FactorK);
            for (int i = offset + 1; i < offset + count && i < models.Length; i++)
                calcProfitRate(models[i], models[i - 1], _param.FactorK);
        }

        static void calcProfitRate(CandleModel model, CandleModel prev, decimal k)
        {
            model.Target = Math.Round(model.Opening + prev.Delta * k, 2);
            model.Rate = (model.High > model.Target && model.Opening >= prev.MovingAvg)
                ? Math.Round(model.Closing / model.Target - CandleModel.FeeRate, 4)
                : 1.0m;
        }

    }//class

}
