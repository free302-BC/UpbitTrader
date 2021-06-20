﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic.Calc
{
    public partial interface ICalc
    {
        /// <summary>
        /// ICalcModel[]에 대한 cumulated profit rate 계산
        /// </summary>
        /// <typeparam name="VM"></typeparam>
        /// <param name="models"></param>
        /// <returns></returns>
        static decimal CalcCumRate<VM>(VM[] models) where VM : ICalcModel
        {
            //-------------------------------------------------------------------------
            // TODO: 호출자가 이 결과를 모아 누적계산시: seed는 중복 계산된다.
            //var seed = offset > 0 ? models[offset - 1].CumRate : 1m;
            //-------------------------------------------------------------------------

            var values = models.Select(v => v.Rate).ToArray();
            var cumRate = calcCumRate(values);
            for (int i = 0; i < values.Length; i++) models[i].CumRate = values[i];
            return cumRate;
        }
        static void CalcCumRate<VM>(VM[] models, int index) where VM : ICalcModel
        {
            var values = models.Select(v => v.Rate).ToArray();
            var seed = index <= 0 ? 1.0m : models[index - 1].CumRate;
            calcCumRate(values, index, seed);//return = 5자리 정밀도
            models[index].CumRate = values[index];//4자리 정밀도
        }

        private static decimal calcCumRate(decimal[] values)
        {
            var cumRate = 1.0m;
            for (int i = 0; i < values.Length; i++)
            {
                cumRate = calcCumRate(values, i, cumRate);
            }
            return Math.Round(cumRate, 4);
        }

        /// <summary>
        /// 'values'에 계산된 cumRate를 넣는다.
        /// </summary>
        /// <param name="values">array of 'Rate' values</param>
        /// <returns>cumRate with .5 digits</returns>
        private static decimal calcCumRate(decimal[] values, int index, decimal seed)
        {
            var cumRate = Math.Round(seed * values[index], 5);
            values[index] = Math.Round(cumRate, 4);
            return cumRate;
        }


        /// <summary>
        /// ICalcModel[]에 대한 cumulated profit rate 계산
        /// </summary>
        /// <typeparam name="VM"></typeparam>
        /// <param name="models"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        static decimal CalcDrawDown<VM>(VM[] models) where VM : ICalcModel
        {
            //-------------------------------------------------------------------------
            //TODO: max 구하기? 현재영역 or 전체
            // 현재영역의 mdd는 의미가 없음 ~ 전체 구간에서 다시 구해야 함.
            //-------------------------------------------------------------------------
            var values = models.Select(v => v.CumRate).ToArray();
            var mdd = CalcDrawDown(values);
            for (int i = 0; i < values.Length; i++) models[i].DrawDown = values[i];
            return mdd;
        }

        static decimal CalcDrawDown(decimal[] models)
        {
            var max = decimal.MinValue;
            for (int i = 0; i < models.Length; i++)
            {
                int j = i;
                max = max > models[j] ? max : models[j];
                models[j] = max > models[j] ? Math.Round(100 * (max - models[j]) / max, 2) : 0m;
            }
            return Math.Round((max - 1) * 100, 2);
        }
    }

}