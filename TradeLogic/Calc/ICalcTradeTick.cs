using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic.Calc
{
    using M = TradeTickModel;

    /// <summary>
    /// ICandle 모델에 대한 계산 알고리즘 구현
    /// </summary>
    public interface ICalcTradeTick
    {
        #region ---- Profit Rate ----

        /// <summary>
        /// 주어진 ICalcParam을 이용하여 Profit Rate를 계산
        /// </summary>
        /// <param name="models"></param>
        /// <param name="param"></param>
        public static void CalcProfitRate(M[] models, ICalcParam param)
            => CalcProfitRate(models, 0, models.Length, param);

        /// <summary>
        /// 주어진 ICalcParam을 이용하여 [offset..count]구간의 Profit Rate를 계산
        /// </summary>
        /// <param name="models"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="param"></param>
        public static void CalcProfitRate(M[] models, int offset, int count, ICalcParam param)
        {
            CalcProfitRate(models[offset], offset > 0
                ? models[offset - 1]
                : M.Empty, param);

            for (int i = 1; i < count; i++)
            {
                var j = offset + i;
                if (j >= models.Length) break;
                CalcProfitRate(models[j], models[j - 1], param);
            }
        }

        /// <summary>
        /// 주어진 K factor에 따라 model의 Profit Rate를 계산
        /// </summary>
        /// <param name="model"></param>
        /// <param name="prev"></param>
        /// <param name="param"></param>
        public static void CalcProfitRate(M model, M prev, ICalcParam param)
        {

        }
        #endregion


        #region ---- MACD OSC ----
        public static void CalcMacdOsc(M[] models, ICalcParam param)
        {
            ICalc.CalcMacdOsc(models, 0, models.Length, param, m => m.UnitPrice);
        }
        #endregion


        #region ---- Moving Average ----

        /// <summary>
        /// 가변 Window Size 기법으로 Moving Average 구함
        /// 데이터 갯수가 부족할 경우 ~ 그 갯수==Window Size
        /// </summary>
        /// <param name="models"></param>
        /// <param name="windowSize"></param>
        /// <param name="winFunc"></param>
        public static void CalcMovingAvg(M[] models, ICalcParam param)
        {
            CalcMovingAvg(models, 0, models.Length, param);
        }
        public static void CalcMovingAvg(M[] models, int offset, int count, ICalcParam param)
        {
            ICalc.CalcMovingAvg(models, offset, count, param, m => m.UnitPrice);
        }
        #endregion


        #region ---- Cumulated Profit Rate & DrawDown ----
        public static decimal CalcCumRate(M[] models)
            => CalcCumRate(models, 0, models.Length);

        public static decimal CalcCumRate(M[] models, int offset, int count)
            => ICalc.CalcCumRate(models, offset, count, m => m.Rate);

        public static decimal CalcDrawDown(M[] models)
            => CalcDrawDown(models, 0, models.Length);
        public static decimal CalcDrawDown(M[] models, int offset, int count)
            => ICalc.CalcDrawDown(models, offset, count, m => m.CumRate);

        #endregion

    }

}
