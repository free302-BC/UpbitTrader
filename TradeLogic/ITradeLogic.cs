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
        void CalcProfitRate(CandleModel[] models, decimal k);

        /// <summary>
        /// models[offset..offset+count]
        /// </summary>
        /// <param name="models"></param>
        /// <param name="k"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        void CalcProfitRate(CandleModel[] models, decimal k, int offset, int count);

        public static void CalcMovingAvg(CandleModel[] models, int size)
        {
            for (int i = 0; i < models.Length; i++)
            {
                var sum = 0m;
                int count = i + 1 >= size ? size : i + 1;
                for (int j = i; j > (i - count); j--) sum += models[j].Closing;
                models[i].MovingAvg = sum / count;
            }
        }

        public static decimal CalcCumRate(CandleModel[] models) => CalcCumRate(models, 0, models.Length);
        public static decimal CalcCumRate(CandleModel[] models, int offset, int count)
        {
            var seed = offset > 0 ? models[offset - 1].CumRate : 1m;
            var rate =  models.Skip(offset).Take(count).Aggregate(seed, (cr, m) => m.CumRate = Math.Round(cr *= m.Rate, 4));
            return Math.Round(rate, 4);
        }

        public static decimal CalcDrawDown(CandleModel[] models) => CalcDrawDown(models, 0, models.Length);
        public static decimal CalcDrawDown(CandleModel[] models, int offset, int count)
        {
            //TODO: max 구하기? 현재영역 or 전체
            var max = decimal.MinValue;
            var sub = models.Skip(offset).Take(count);
            foreach (var m in sub)
            {
                max = max > m.CumRate ? max : m.CumRate;
                m.DrawDown = max > m.CumRate ? Math.Round(100 * (max - m.CumRate) / max, 2) : 0m;
            }
            return sub.Max(m => m.DrawDown);
        }


        //void calcRate_StopLoss(CandleModel prev, decimal k)
        //{
        //    Target = Math.Round(Opening + prev.Delta * k, 2);

        //    var sellPrice = Target * 0.99m > Low ? Math.Max(Target * 0.985m, Low) : Closing;
        //    //하락후 회복시 미반영

        //    Rate = (High > Target) ? Math.Round(sellPrice / Target - FeeRate, 4) : 1.0m;
        //}

    }
}
