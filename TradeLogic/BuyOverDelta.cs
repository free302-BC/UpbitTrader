using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Analysis;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic
{
    public class BuyOverDelta : ITradeLogic
    {
        public void CalcProfitRate(CandleModel[] models, decimal k) 
            => CalcProfitRate(models, k, 0, models.Length);

        public void CalcProfitRate(CandleModel[] models, decimal k, int offset, int count)
        {
            calcProfitRate(models[offset], offset > 0 ? models[offset - 1] : CandleModel.Empty, k);
            for (int i = offset + 1; i < offset + count && i < models.Length; i++)
                calcProfitRate(models[i], models[i - 1], k);
        }

        void calcProfitRate(CandleModel model, CandleModel prev, decimal k)
        {
            model.Target = Math.Round(model.Opening + prev.Delta * k, 2);
            model.Rate = (model.High > model.Target) 
                ? Math.Round(model.Closing / model.Target - CandleModel.FeeRate, 4) 
                : 1.0000m;
        }

        public static BuyOverDelta Default = new();

    }//class

    public class BuyOverDeltaMA : ITradeLogic
    {
        public void CalcProfitRate(CandleModel[] models, decimal k) 
            => CalcProfitRate(models, k, 0, models.Length);

        public void CalcProfitRate(CandleModel[] models, decimal k, int offset, int count)
        {
            ITradeLogic.CalcMovingAvg(models, 5);

            calcProfitRate(models[offset], offset > 0 ? models[offset - 1] : CandleModel.Empty, k);
            for (int i = offset + 1; i < offset + count && i < models.Length; i++)
                calcProfitRate(models[i], models[i - 1], k);
        }

        static void calcProfitRate(CandleModel model, CandleModel prev, decimal k)
        {
            model.Target = Math.Round(model.Opening + prev.Delta * k, 2);
            model.Rate = (model.High > model.Target && model.Opening >= model.MovingAvg)
                ? Math.Round(model.Closing / model.Target - CandleModel.FeeRate, 4)
                : 1.0m;
        }

        public static BuyOverDeltaMA Default = new();

    }//class

}
