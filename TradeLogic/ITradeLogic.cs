using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic
{
    public interface ITradeLogic
    {
        void CalcProfitRate(IList<CandleModel> models, decimal k);

        public static decimal CalcCumRate(IList<CandleModel> models)
        {
            var finalRate = models.Aggregate(1.0m, (cr, m) => m.CumRate = Math.Round(cr *= m.Rate, 4));
            return Math.Round(finalRate, 4);
        }
        public static decimal CalcDrawDown(IList<CandleModel> models)
        {
            var max = decimal.MinValue;
            foreach (var m in models)
            {
                max = max > m.CumRate ? max : m.CumRate;
                m.DrawDown = max > m.CumRate ? Math.Round(100 * (max - m.CumRate) / max, 2) : 0m;
            }
            return models.Max(m => m.DrawDown);
        }


    }



}
