using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// <param name="param"></param>
        /// <param name="getter"></param>
        static void CalcMacd<VM>(VM[] models, ICalcParam param) where VM : ICalcModel
        {
            var values = models.Select(v => v.Value).ToArray();
            var results = CalcMacd(values, param.MacdParam);
            var osc = results.osc;
            var macd = results.macd;
            for (int i = 0; i < osc.Length; i++)
            {
                models[i].Macd = results.macd[i];
                models[i].MacdOsc = results.osc[i];
            }
        }

        static void CalcMacd<VM>(VM[] models, ICalcParam param, int index) where VM : ICalcModel
        {
            var values = models.Select(v => v.Value).ToArray();
            var macds = models.Select(v => v.Macd).ToArray();
            var results = CalcMacd(values, macds, param.MacdParam, index);
            models[index].Macd = results.macd;
            models[index].MacdOsc = results.osc;
        }

        /// <summary>
        /// values의 [0..]에 대한 macd를 index=0부터 순차적으로 계산
        /// </summary>
        /// <param name="values"></param>
        /// <param name="windowSizes"></param>
        /// <returns></returns>
        static (decimal[] ma8, decimal[] ma16, decimal[] macd, decimal[] signal, decimal[] osc)
            CalcMacd(decimal[] values, int[] windowSizes)
        {
            var results = new decimal[5][];
            for (int i = 0; i < 5; i++) results[i] = new decimal[values.Length];

            for (int i = 0; i < values.Length; i++)
                (results[0][i], results[1][i], results[2][i], results[3][i], results[4][i])
                    = CalcMacd(values, results[2], windowSizes, i);

            return (results[0], results[1], results[2], results[3], results[4]);
        }

        /// <summary>
        /// values의 index위치 macd계산, index이전 macd 필요
        /// </summary>
        /// <param name="values"></param>
        /// <param name="macds"></param>
        /// <param name="windowSizes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        static (decimal ma8, decimal ma16, decimal macd, decimal signal, decimal osc)
            CalcMacd(in decimal[] values, in decimal[] macds, int[] windowSizes, int index)
        {
            var winFuncs = _winFuncs[WindowFunction.Gaussian];

            var avg8 = CalcMovingAvg(values, winFuncs[Math.Min(index + 1, windowSizes[0])], index);
            var avg16 = CalcMovingAvg(values, winFuncs[Math.Min(index + 1, windowSizes[1])], index);
            var macd = macds[index] = avg8 - avg16;//TODO: macds[index] = macd의 부수효과 검토
            var signal = CalcMovingAvg(macds, winFuncs[Math.Min(index + 1, windowSizes[2])], index);
            var osc = macd - signal;

            return (avg8, avg16, macd, signal, osc);
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
        private static decimal[] _macd =
        {
            0.00m,0.00m,0.00m,0.00m,0.00m,0.00m,0.00m,0.00m,-0.02m,-0.08m,-0.15m,
            -0.33m,-0.28m,-0.27m,-0.25m,-0.26m,-0.23m,-0.28m,-0.30m,-0.28m,-0.32m,
            -0.34m,-0.36m,-0.36m,-0.35m,-0.35m,-0.39m,-0.40m,-0.42m,-0.60m,-0.73m,
            -0.85m,-0.56m,-0.23m,-0.27m,-0.33m,-0.42m,-0.05m,-0.11m,-0.19m,-0.37m,
            -0.51m,-0.49m,-0.57m,-0.52m,-0.7m,-0.76m,-0.84m,-0.81m,-0.68m
        };

        public static void MacdTest()
        {
            int[] windowSizes = { 8, 16, 5 };

            var ma = CalcMacd(_src, windowSizes);
            save("macd.txt", ma);

            var ma2 = CalcMacd(_src, windowSizes);
            save("macd_1by1.txt", ma2);

            var watch = Stopwatch.StartNew();
            for (int i = 0; i < 100000; i++) CalcMacd(_src, windowSizes);
            var dt1 = watch.ElapsedMilliseconds;

            watch.Restart();
            for (int i = 0; i < 100000; i++) CalcMacd(_src, windowSizes);
            var dt2 = watch.ElapsedMilliseconds;

            Debug.WriteLine($"calcMacd: {dt1} ~ {dt2} ms");//8461 ~ 8623
            File.WriteAllText("macd_timing.txt", $"{dt1}\n{dt2}");

            void save(string fn, (decimal[] ma8, decimal[] ma16, decimal[] macd, decimal[] signal, decimal[] osc) v)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"src\tma8\tma16\tmacd\tsignal\tosc");
                for (int i = 0; i < _src.Length; i++)
                    sb.AppendLine(
                        $"{_src[i]:F2}\t{v.ma8[i],6:F2}\t{v.ma16[i],6:F2}" +
                        $"\t{v.macd[i],6:F2}\t{v.signal[i],6:F2}\t{v.osc[i],6:F2}");
                File.WriteAllText(fn, sb.ToString());
            }
        }
        #endregion

    }
}
