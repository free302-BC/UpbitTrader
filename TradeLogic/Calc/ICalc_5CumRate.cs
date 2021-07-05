using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
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
            var seed = 1.00000m;
            for (int i = 0; i < models.Length; i++)
            {
                seed *= models[i].Rate;
                models[i].CumRate = Math.Round(seed, 5);
            }
            return Math.Round(seed, 5);
        }
        static void CalcCumRate<VM>(VM[] models, int index) where VM : ICalcModel
        {
            var values = models.Select(v => v.Rate).ToArray();
            var seed = index <= 0 ? 1.00000m : models[index - 1].CumRate;
            models[index].CumRate = Math.Round(seed * values[index], 5);//calcCumRate(values, index, seed);
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
