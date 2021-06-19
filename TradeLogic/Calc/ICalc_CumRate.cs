using System;
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
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        static decimal CalcCumRate<VM>(VM[] models, int offset, int count, Func<VM, decimal> getter)
            where VM : ICalcModel
        {
            //-------------------------------------------------------------------------
            // TODO: 호출자가 이 결과를 모아 누적계산시: seed는 중복 계산된다.
            //var seed = offset > 0 ? models[offset - 1].CumRate : 1m;
            //-------------------------------------------------------------------------

            var values = models.Select(v => getter(v)).ToArray();
            var cumRate = calcCumRate(values, offset, count);
            for (int i = 0; i < values.Length; i++) models[i].CumRate = values[i];
            return cumRate;
        }

        private static decimal calcCumRate(decimal[] models, int offset, int count)
        {
            var cumRate = 1.0m;
            for (int i = 0; i < count; i++)
            {
                int j = offset + i;
                cumRate = Math.Round(cumRate * models[j], 5);
                models[j] = Math.Round(cumRate, 4);
            }
            return Math.Round(cumRate, 4);
        }

        /// <summary>
        /// ICalcModel[]에 대한 cumulated profit rate 계산
        /// </summary>
        /// <typeparam name="VM"></typeparam>
        /// <param name="models"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        /// <returns></returns>
        static decimal CalcDrawDown<VM>(VM[] models, int offset, int count, Func<VM, decimal> getter)
            where VM : ICalcModel
        {
            //-------------------------------------------------------------------------
            //TODO: max 구하기? 현재영역 or 전체
            // 현재영역의 mdd는 의미가 없음 ~ 전체 구간에서 다시 구해야 함.
            //-------------------------------------------------------------------------
            var values = models.Select(v => getter(v)).ToArray();
            var mdd = CalcDrawDown(values, offset, count);
            for (int i = 0; i < values.Length; i++) models[i].DrawDown = values[i];
            return mdd;
        }

        static decimal CalcDrawDown(decimal[] models, int offset, int count)
        {
            var max = decimal.MinValue;
            for (int i = 0; i < count; i++)
            {
                int j = offset + i;
                max = max > models[j] ? max : models[j];
                models[j] = max > models[j] ? Math.Round(100 * (max - models[j]) / max, 2) : 0m;
            }
            return Math.Round((max - 1) * 100, 2);
        }
    }

}
