using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic.Calc
{
    /// <summary>
    /// ICandle 모델에 대한 계산 알고리즘 구현
    /// </summary>
    public interface ICalcCandle
    {
        public static void CalcMacdOsc(CandleModel[] models, ICalcParam param)
        {
            ICalc.CalcMacdOsc(models, 0, models.Length, param, m => m.Closing, (m, d) => m.MacdOsc = d);
        }

        #region ---- Moving Average ----

        /// <summary>
        /// 가변 Window Size 기법으로 Moving Average 구함
        /// 데이터 갯수가 부족할 경우 ~ 그 갯수==Window Size
        /// </summary>
        /// <param name="models"></param>
        /// <param name="windowSize"></param>
        /// <param name="winFunc"></param>
        public static void CalcMovingAvg(CandleModel[] models, ICalcParam param)
        {
            CalcMovingAvg(models, 0, models.Length, param);
        }
        public static void CalcMovingAvg(CandleModel[] models, int offset, int count, ICalcParam param)
        {
            ICalc.CalcMovingAvg(models, offset, count, param, m => m.Closing, (m, ma) => m.MovingAvg = ma);
        }
        #endregion


        #region ---- Cumulated Profit Rate & DrawDown ----
        public static decimal CalcCumRate(CandleModel[] models) => CalcCumRate(models, 0, models.Length);
        public static decimal CalcCumRate(CandleModel[] models, int offset, int count)
        {
            //-------------------------------------------------------------------------
            // TODO: 호출자가 이 결과를 모아 누적계산시: seed는 중복 계산된다.
            //var seed = offset > 0 ? models[offset - 1].CumRate : 1m;
            //-------------------------------------------------------------------------

            var rate = models.Skip(offset).Take(count).Aggregate(1m, (cr, m) => m.CumRate = Math.Round(cr *= m.Rate, 4));
            return Math.Round(rate, 4);
        }
        public static decimal CalcDrawDown(CandleModel[] models) => CalcDrawDown(models, 0, models.Length);
        public static decimal CalcDrawDown(CandleModel[] models, int offset, int count)
        {
            //-------------------------------------------------------------------------
            //TODO: max 구하기? 현재영역 or 전체
            // 현재영역의 mdd는 의미가 없음 ~ 전체 구간에서 다시 구해야 함.
            //-------------------------------------------------------------------------

            var max = decimal.MinValue;
            var sub = models.Skip(offset).Take(count);
            foreach (var m in sub)
            {
                max = max > m.CumRate ? max : m.CumRate;
                m.DrawDown = max > m.CumRate ? Math.Round(100 * (max - m.CumRate) / max, 2) : 0m;
            }

            return sub.Count() > 0 ? sub.Max(m => m.DrawDown) : 0m;
        }
        #endregion

    }

}
