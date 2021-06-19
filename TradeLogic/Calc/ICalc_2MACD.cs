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
        static void CalcMacdOsc<VM>(VM[] models, ICalcParam param, Func<VM, decimal> getter)
            where VM : ICalcModel
        {
            var values = models.Select(v => getter(v)).ToArray();
            var osc = calcMacdOsc(values, param.MacdParam);
            for (int i = 0; i < osc.Length; i++) models[i].MacdOsc = osc[i];
        }

        private static decimal[] calcMacdOsc(decimal[] values, int[] windowSizes)
        {
            return calcMacd(values, windowSizes).osc;
        }

        private static (decimal[] ma8, decimal[] ma16, decimal[] macd, decimal[] signal, decimal[] osc)
            calcMacd(decimal[] values, int[] windowSizes)
        {
            var wf = WindowFunction.Gaussian;
            //int[] windowSizes = { 8, 16, 5 };
            decimal[][] avgs = new decimal[2][];

            for (int i = 0; i < 2; i++)
                avgs[i] = calcMovingAvg(values, windowSizes[i], wf);

            var macd = avgs[0].Zip(avgs[1], (x, y) => x - y).ToArray();
            var signal = calcMovingAvg(macd, windowSizes[2], wf);
            var osc = macd.Zip(signal, (m, s) => m - s).ToArray();

            return (avgs[0], avgs[1], macd, signal, osc);
        }

        private static (decimal ma8, decimal ma16, decimal macd, decimal signal, decimal osc)
            calcMacd(decimal[] values, int[] windowSizes, int index)
        {
            var wf = WindowFunction.Gaussian;
            var winFuncs = _winFuncs[WindowFunction.Gaussian];

            decimal[] avgs = new decimal[2];
            avgs[0] = calcMovingAvg(values, winFuncs[Math.Min(index + 1, windowSizes[0])], index);
            avgs[1] = calcMovingAvg(values, winFuncs[Math.Min(index + 1, windowSizes[1])], index);

            var macd = avgs[0] - avgs[1];            

            //ma of macd : macd array필요...
            var signal = calcMovingAvg(macd, winFuncs[Math.Min(index + 1, windowSizes[1])], index);
            var osc = macd.Zip(signal, (m, s) => m - s).ToArray();

            return (avgs[0], avgs[1], macd, signal, osc);
        }



        #region ---- TEST ----

        private static decimal[] _src =
        {
            4340.3m,4340.3m,4340.5m,4345.8m,4340.3m,4340.3m,4340.3m,4340.4m,4340.2m,
            4340.2m,4340.0m,4340.4m,4340.3m,4340.0m,4340.0m,4340.0m,4340.0m,4339.2m,
            4339.4m,4339.1m,4339.1m,4339.1m,4338.9m,4338.9m,4338.9m,4338.4m,4338.1m,
            4338.0m,4338.0m,4336.0m,4336.2m,4335.9m,4339.9m,4339.9m,4335.9m,4335.9m,
            4335.9m,4340.3m,4335.8m,4335.9m,4335.8m,4335.8m,4335.8m,4334.8m,4335.9m,
            4334.8m,4333.9m,4333.7m,4333.9m,4334.8m,
        };

        public static void MacdTest()
        {
            int[] windowSizes = { 8, 16, 5 };
            var ma = calcMacd(_src, windowSizes);
            save();

            void save()
            {
                var sb = new StringBuilder();
                sb.AppendLine($"src\tma8\tma16\tmacd\tsignal\tosc");
                for (int i = 0; i < _src.Length; i++)
                    sb.AppendLine(
                        $"{_src[i]:F2}\t{ma.ma8[i],6:F2}\t{ma.ma16[i],6:F2}" +
                        $"\t{ma.macd[i],6:F2}\t{ma.signal[i],6:F2}\t{ma.osc[i],6:F2}");
                File.WriteAllText("macd.txt", sb.ToString());
            }
        }
        #endregion
    }
}
