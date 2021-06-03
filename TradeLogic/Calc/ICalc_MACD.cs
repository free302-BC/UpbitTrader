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
        static void CalcMacdOsc<VM>(VM[] models, int offset, int count, ICalcParam param,
            Func<VM, decimal> getter, Action<VM, decimal> setter)
            where VM : IViewModel
        {
            var values = models.Select(v => getter(v)).ToArray();
            var osc = CalcMacdOsc(values, offset, count, param.MacdWindowSizes);
            for (int i = 0; i < osc.Length; i++) setter(models[i], osc[i]);
        }

        static decimal[] CalcMacdOsc(decimal[] values, int offset, int count, int[] windowSizes)
        {
            return calcMacd(values, offset, count, windowSizes).osc;
        }

        private static (decimal[] ma8, decimal[] ma16, decimal[] macd, decimal[] signal, decimal[] osc)
            calcMacd(decimal[] models, int offset, int count, int[] windowSizes)
        {
            var wf = WindowFunction.Gaussian;
            //int[] windowSizes = { 8, 16, 5 };
            decimal[][] avgs = new decimal[2][];

            for (int i = 0; i < 2; i++)
                avgs[i] = CalcMovingAvg(models, offset, count, windowSizes[i], wf);

            var macd = avgs[0].Zip(avgs[1], (x, y) => x - y).ToArray();
            var signal = CalcMovingAvg(macd, 0, macd.Length, windowSizes[2], wf);
            var osc = macd.Zip(signal, (m, s) => m - s).ToArray();

            return (avgs[0], avgs[1], macd, signal, osc);
        }

        private static decimal[] _src =
        {
            0.67m, 0.08m, 0.04m, 0.49m, 0.06m, 0.30m, 0.20m, 0.22m, 0.07m, 0.08m,
            0.67m, 0.61m, 0.68m, 0.59m, 1.37m, 0.86m, 1.37m, 0.74m, 0.79m, 0.53m,
            0.12m, 0.04m, 0.50m, 0.31m, 0.73m, 1.04m, 0.79m, 0.88m, 0.28m, 1.41m
        };

        public static void MacdTest()
        {
            int[] windowSizes = { 8, 16, 5 };
            var ma = calcMacd(_src, 0, _src.Length, windowSizes);
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
    }

}
