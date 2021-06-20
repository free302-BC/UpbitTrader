using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;
using M = Universe.Coin.TradeLogic.Model.ICalcModel;

namespace Universe.Coin.TradeLogic.Calc
{
    public partial interface ICalc
    {
        /// <summary>
        /// 주어진 ICalcParam을 이용하여 Profit Rate를 계산
        /// </summary>
        /// <param name="models"></param>
        /// <param name="param"></param>
        public void CalcProfitRate(M[] models, ICalcParam param)
        {
            //calcSignal(models[0], M.Empty, param);
            for (int i = 1; i < models.Length; i++)
                CalcSignal(models[i], models[i - 1], param);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CalcProfitRate(M[] models, ICalcParam param, int index)
        {
            if (index <= 0) return;
            CalcSignal(models[index], models[index - 1], param);
        }

        protected void CalcSignal(M model, M prev, ICalcParam param);



        #region ---- TEST ----

        public static void MacdProfitRate()
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
