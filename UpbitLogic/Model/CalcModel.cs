using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit.Model
{
    public class CalcModel
    {
        public DateTime DateKST;
        public double Opening;
        public double High;
        public double Low;
        public double Closing;
        public double Delta;

        public double Target;
        public double Rate;
        public double CumRate;
        public double DrawDown;
        const double _feeRate = 0.0005 * 2;

        public static CalcModel Default = new CalcModel();
        CalcModel() 
        {
            DateKST = DateTime.MinValue;
            Opening = High = Low = Closing = Delta = double.NaN;
        }
        public CalcModel(ICandle candle)
        {
            DateKST = DateTime.Parse(candle.CandleDateTimeKst);
            Opening = Math.Round(candle.OpeningPrice / 10000.0, 1);
            High = Math.Round(candle.HighPrice / 10000.0, 1);
            Low = Math.Round(candle.LowPrice / 10000.0, 1);
            Closing = Math.Round(candle.TradePrice / 10000.0, 1);
            Delta = High - Low;
        }
        void calcRate(CalcModel prev, double k)
        {
            Target = Math.Round(Opening + prev.Delta * k, 2);
            Rate = (High > Target) ? Math.Round(Closing / Target - _feeRate, 4) : 1.0;
        }
        public override string ToString() 
            => $"{DateKST:yyMMdd.HHmm} {Opening,8:F1} {Target,8:F1} {High,8:F1} {Closing,8:F1} : {Rate,8:F4} {CumRate,8:F4} {DrawDown,8:F2}";

        public static void CalcRate(IList<CalcModel> models, double k)
        {
            models.Insert(0, Default);
            for (int i = 1; i < models.Count; i++) models[i].calcRate(models[i - 1], k);
            models.RemoveAt(0);
        }
        public static double CalcCumulativeRate(IList<CalcModel> models)
        {
            var totalRate = models.Aggregate(1.0, (t, m) => m.CumRate = Math.Round(t *= m.Rate, 4));
            return Math.Round(totalRate, 4);
        }
        static void calcDrawdown(IList<CalcModel> models)
        {

        }

        public static string Print(IEnumerable<CalcModel> models)
        {
            var sb = new StringBuilder();
            sb.AppendLine(
                $"{_names[0],11} {_names[1],8} {_names[2],8} {_names[3],8} {_names[4],8} : {_names[5],8} {_names[6],8} {_names[7],8}");
            foreach (var m in models) sb.AppendLine(m.ToString());
            return sb.ToString();
        }
        static string[] _names =
        {
            nameof(DateKST), nameof(Opening), nameof(Target), nameof(High), nameof(Closing), nameof(Rate),
            nameof(CumRate), nameof(DrawDown)
        };


    }//class
}
