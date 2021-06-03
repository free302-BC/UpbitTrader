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
        #region ---- Profit Rate ----

        /// <summary>
        /// 주어진 ICalcParam을 이용하여 Profit Rate를 계산
        /// </summary>
        /// <param name="models"></param>
        /// <param name="param"></param>
        public static void CalcProfitRate(CandleModel[] models, ICalcParam param)
            => CalcProfitRate(models, 0, models.Length, param);

        /// <summary>
        /// 주어진 ICalcParam을 이용하여 [offset..count]구간의 Profit Rate를 계산
        /// </summary>
        /// <param name="models"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="param"></param>
        public static void CalcProfitRate(CandleModel[] models, int offset, int count, ICalcParam param)
        {
            CalcProfitRate(models[offset], offset > 0
                ? models[offset - 1]
                : CandleModel.Empty, param);

            for (int i = offset + 1; i < offset + count && i < models.Length; i++)
                CalcProfitRate(models[i], models[i - 1], param);
        }

        /// <summary>
        /// 주어진 K factor에 따라 model의 Profit Rate를 계산
        /// </summary>
        /// <param name="model"></param>
        /// <param name="prev"></param>
        /// <param name="param"></param>
        public static void CalcProfitRate(CandleModel model, CandleModel prev, ICalcParam param)
        {
            var k = param.FactorK;
            model.Target = Math.Round(model.Opening + prev.Delta * k, 2);

            var doTrade = param.WindowFunction == WindowFunction.None
                ? (model.High > model.Target)
                : (model.High > model.Target && model.Opening >= prev.MovingAvg);

            model.Rate = doTrade
                    ? Math.Round(model.Closing / model.Target - CandleModel.FeeRate, 4)
                    : 1.0000m;
        }
        #endregion


        #region ---- MACD OSC ----
        public static void CalcMacdOsc(CandleModel[] models, ICalcParam param)
        {
            ICalc.CalcMacdOsc(models, 0, models.Length, param, m => m.Closing);
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
        public static void CalcMovingAvg(CandleModel[] models, ICalcParam param)
        {
            CalcMovingAvg(models, 0, models.Length, param);
        }
        public static void CalcMovingAvg(CandleModel[] models, int offset, int count, ICalcParam param)
        {
            ICalc.CalcMovingAvg(models, offset, count, param, m => m.Closing);
        }
        #endregion


        #region ---- Cumulated Profit Rate & DrawDown ----
        public static decimal CalcCumRate(CandleModel[] models) 
            => CalcCumRate(models, 0, models.Length);

        public static decimal CalcCumRate(CandleModel[] models, int offset, int count) 
            => ICalc.CalcCumRate(models, offset, count, m => m.Rate);

        public static decimal CalcDrawDown(CandleModel[] models) 
            => CalcDrawDown(models, 0, models.Length);
        public static decimal CalcDrawDown(CandleModel[] models, int offset, int count) 
            => ICalc.CalcDrawDown(models, offset, count, m => m.CumRate);

        #endregion

    }

}
