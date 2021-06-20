using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic.Calc
{
    using M = CandleModel;

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
        public static void CalcProfitRate(M[] models, ICalcParam param)
        {
            CalcProfitRate(models[0], M.Empty, param);
            for (int i = 1; i < models.Length; i++)
                CalcProfitRate(models[i], models[i - 1], param);
        }

        /// <summary>
        /// 주어진 K factor에 따라 model의 Profit Rate를 계산
        /// </summary>
        /// <param name="model"></param>
        /// <param name="prev"></param>
        /// <param name="param"></param>
        public static void CalcProfitRate(M model, M prev, ICalcParam param)
        {
            var k = param.FactorK;
            model.Target = Math.Round(model.Opening + prev.Delta * k, 2);

            var doTrade = param.WindowFunction == WindowFunction.None
                ? (model.High > model.Target)
                : (model.High > model.Target && model.Opening >= prev.MovingAvg);

            model.TradeDone = doTrade;
            model.Rate = doTrade
                    ? Math.Round(model.Closing / model.Target - M.FeeRate, 4)
                    : 1.0000m;
        }
        #endregion


        #region ---- MACD OSC ----
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalcMacdOsc(M[] models, ICalcParam param) 
            => ICalc.CalcMacd(models, param);
        #endregion


        #region ---- Moving Average ----

        /// <summary>
        /// 가변 Window Size 기법으로 Moving Average 구함
        /// 데이터 갯수가 부족할 경우 ~ 그 갯수==Window Size
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalcMovingAvg(M[] models, ICalcParam param) 
            => ICalc.CalcMovingAvg(models, param);
        #endregion


        #region ---- Cumulated Profit Rate & DrawDown ----

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal CalcCumRate(M[] models) 
            => ICalc.CalcCumRate(models);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal CalcDrawDown(M[] models) 
            => ICalc.CalcDrawDown(models);

        #endregion

    }

}
