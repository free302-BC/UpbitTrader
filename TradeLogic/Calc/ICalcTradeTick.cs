﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        #region ---- Moving Average ----

        /// <summary>
        /// 가변 Window Size 기법으로 Moving Average 구함
        /// 데이터 갯수가 부족할 경우 ~ 그 갯수==Window Size
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalcMovingAvg(M[] models, ICalcParam param) 
            => ICalc.CalcMovingAvg(models, param);
        #endregion


        #region ---- MACD OSC ----

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalcMacdOsc(M[] models, ICalcParam param) 
            => ICalc.CalcMacd(models, param);
        #endregion


        #region ---- Profit Rate ----

        /// <summary>
        /// 주어진 ICalcParam을 이용하여 Profit Rate를 계산
        /// </summary>
        /// <param name="models"></param>
        /// <param name="param"></param>
        public static void CalcProfitRate(M[] models, ICalcParam param)
        {
            calcSignal(models[0], M.Empty, param);
            for (int i = 1; i < models.Length; i++)
                calcSignal(models[i], models[i - 1], param);
        }

        /// <summary>
        /// MACD에 따라 TimingSignal 계산
        /// </summary>
        /// <param name="model"></param>
        /// <param name="prev"></param>
        /// <param name="param"></param>
        static void calcSignal(M model, M prev, ICalcParam param)
        {
            //chekc buy | sell | nop
            var signal = TimingSignal.N;
            if (prev.MacdOsc > param.BuyMacd) signal = TimingSignal.Buy;
            if (prev.MacdOsc < param.SellMacd) signal = TimingSignal.Sell;

            model.Signal = prev.Signal switch
            {
                TimingSignal.N or TimingSignal.Sell
                    => signal == TimingSignal.Buy ? signal : TimingSignal.N,
                _ => signal == TimingSignal.Sell ? signal : TimingSignal.Hold,
            };
            //
            model.TradeDone = model.Signal == TimingSignal.Hold || model.Signal == TimingSignal.Sell;

            //
            if (model.TradeDone)
            {
                model.Rate = model.UnitPrice / prev.UnitPrice;
                if (model.Signal == TimingSignal.Buy || model.Signal == TimingSignal.Sell)
                    model.Rate -= M.FeeRate;
                model.Rate = Math.Round(model.Rate, 4);
            }
        }


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
