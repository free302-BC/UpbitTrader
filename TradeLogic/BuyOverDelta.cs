using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic
{
    public class BuyOverDelta : ITradeLogic
    {
        public void CalcProfitRate(IList<CandleModel> models, decimal k, decimal ma = 0)
        {
            CalcProfitRate(models[0], CandleModel._empty, k);
            for (int i = 1; i < models.Count; i++)
                CalcProfitRate(models[i], models[i - 1], k);
        }
        public void CalcProfitRate(CandleModel model, CandleModel prev, decimal k, decimal ma = 0)
        {
            model.Target = Math.Round(model.Opening + prev.Delta * k, 2);
            model.Rate = (model.High > model.Target) ? Math.Round(model.Closing / model.Target - CandleModel.FeeRate, 4) : 1.0m;
        }
    }

    public class BuyOverDeltaMA : ITradeLogic
    {
        public void CalcProfitRate(IList<CandleModel> models, decimal k, decimal ma = 0)
        {
            CalcProfitRate(models[0], CandleModel._empty, k);
            for (int i = 1; i < models.Count; i++)
                CalcProfitRate(models[i], models[i - 1], k);
        }
        public void CalcProfitRate(CandleModel model, CandleModel prev, decimal k, decimal ma = 0)
        {
            model.Target = Math.Round(model.Opening + prev.Delta * k, 2);
            model.Rate = (model.High > model.Target && model.Opening >= ma)
                ? Math.Round(model.Closing / model.Target - CandleModel.FeeRate, 4)
                : 1.0m;
        }
    }

}
