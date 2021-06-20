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
        /// ICalcModel[]에 대한 MACD oscillator 계산
        /// </summary>
        /// <typeparam name="VM"></typeparam>
        /// <param name="models"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="param"></param>
        /// <param name="getter"></param>
        static void Normalize<VM>(VM[] models, Func<VM, decimal> getter, Action<VM, decimal> setter)
            where VM : IModel
        {
            var values = models.Select(v => getter(v)).ToArray();
            normalize(values);
            for (int i = 0; i < values.Length; i++) setter(models[i], values[i]);
        }

        private static void normalize(decimal[] models)
        {
            var avg = models.Average();
            var sd = (decimal)Math.Sqrt((double)models.Average(x => (x - avg) * (x - avg)));

            for (int i = 0; i < models.Length; i++) models[i] = (models[i] - avg) / sd;
        }

        #region ---- TEST ----

        public static void NormalizedTest()
        {
            var n_src = _src.ToArray();
            normalize(n_src);
            var n_ma = CalcMovingAvg(n_src, 5, WindowFunction.Linear);
            var n_osc = CalcMacd(n_src, new[] { 8, 16, 5 }).osc;

            var sb = new StringBuilder();
            sb.AppendLine($"src\tn-src\tn-ma\tn-osc");
            for (int i = 0; i < n_src.Length; i++)
            {
                sb.AppendLine($"{_src[i]}\t{n_src[i]}\t{n_ma[i]}\t{n_osc[i]}");
            }
            File.WriteAllText("normalized_macd.txt", sb.ToString());
        }
        #endregion
    }
}
