﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic.Calc
{
    /// <summary>
    /// ICandle 모델에 대한 계산 알고리즘 구현
    /// </summary>
    public class CandleCalc : ICalc
    {
        const decimal _FeeRate = 0.0005m * 2;
        public static ICalc I = new CandleCalc();

        /// <summary>
        /// 주어진 K factor에 따라 model의 Profit Rate를 계산
        /// </summary>
        /// <param name="model"></param>
        /// <param name="prev"></param>
        /// <param name="param"></param>
        void ICalc.CalcSignal(ICalcModel model, ICalcModel prev, ICalcParam param)
        {
            var k = param.FactorK;
            var m = (CandleModel)model;

            model.Target = Math.Round(m.Opening + ((CandleModel)prev).Delta * k, 2);

            var doTrade = param.WindowFunction == WindowFunction.None
                ? (m.High > model.Target)
                : (m.High > model.Target && m.Opening >= prev.MovingAvg);

            model.TradeDone = doTrade;
            model.Rate = doTrade ? Math.Round(model.Value / model.Target - _FeeRate, 4) : 1.0000m;
        }

    }//class
}
