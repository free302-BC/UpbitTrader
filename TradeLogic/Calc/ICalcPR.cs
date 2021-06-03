﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic.Calc
{
    /// <summary>
    /// Profit Rate Calcualtor 
    /// </summary>
    //public interface ICalcPR
    //{
    //    /// <summary>
    //    /// 주어진 ICalcParam을 이용하여 Profit Rate를 계산
    //    /// </summary>
    //    /// <param name="models"></param>
    //    /// <param name="param"></param>
    //    public static void CalcProfitRate(CandleModel[] models, ICalcParam param)
    //        => CalcProfitRate(models, 0, models.Length, param);

    //    /// <summary>
    //    /// 주어진 ICalcParam을 이용하여 [offset..count]구간의 Profit Rate를 계산
    //    /// </summary>
    //    /// <param name="models"></param>
    //    /// <param name="offset"></param>
    //    /// <param name="count"></param>
    //    /// <param name="param"></param>
    //    public static void CalcProfitRate(CandleModel[] models, int offset, int count, ICalcParam param)
    //    {
    //        CalcProfitRate(models[offset], offset > 0
    //            ? models[offset - 1]
    //            : CandleModel.Empty, param);

    //        for (int i = offset + 1; i < offset + count && i < models.Length; i++)
    //            CalcProfitRate(models[i], models[i - 1], param);
    //    }

    //    /// <summary>
    //    /// 주어진 K factor에 따라 model의 Profit Rate를 계산
    //    /// </summary>
    //    /// <param name="model"></param>
    //    /// <param name="prev"></param>
    //    /// <param name="param"></param>
    //    public static void CalcProfitRate(CandleModel model, CandleModel prev, ICalcParam param)
    //    {
    //        var k = param.FactorK;
    //        model.Target = Math.Round(model.Opening + prev.Delta * k, 2);

    //        var doTrade = param.WindowFunction == WindowFunction.None
    //            ? (model.High > model.Target)
    //            : (model.High > model.Target && model.Opening >= prev.MovingAvg);

    //        model.Rate = doTrade
    //                ? Math.Round(model.Closing / model.Target - CandleModel.FeeRate, 4)
    //                : 1.0000m;
    //    }
    //}
}