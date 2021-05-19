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

        public string TimeKST = "";
        public string Change = "";

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

        #region ---- Ticker ----
        public CalcModel(Ticker ticker)
        {
            var s = ticker.TradeTimeKst;
            TimeKST = ticker.TradeTimeKst;
            Opening = Math.Round(ticker.OpeningPrice / 10000.0, 1);
            High = Math.Round(ticker.HighPrice / 10000.0, 1);
            Low = Math.Round(ticker.LowPrice / 10000.0, 1);
            Closing = Math.Round(ticker.TradePrice / 10000.0, 1);
            Delta = Math.Round(ticker.SignedChangePrice / 10000.0, 1);
            Change = ticker.Change;
        }
        public string ToTickerString()
            => $"{TimeKST,8} {Opening,8:F1} {High,8:F1} {Low,8:F1} {Closing,8:F1} : {Delta,8:F1} {Change}";
        #endregion

        #region ---- Orderbook ----
        public CalcModel(OrderbookUnit order)
        {
            High = Math.Round(order.AskPrice / 10000.0, 1);
            Low = Math.Round(order.BidPrice / 10000.0, 1);
            Opening = order.AskSize;
            Closing = order.BidSize;
        }
        public double Ask => High;
        public double Bid => Low;
        public double AskAmount => Opening;
        public double BidAmount => Closing;
        public string ToOrderString() => $"{AskAmount,8} {Ask,8:F1} {Bid,8:F1} {BidAmount,8:F1}";
        #endregion

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
        public static double CalcCumRate(IList<CalcModel> models)
        {
            var finalRate = models.Aggregate(1.0, (cr, m) => m.CumRate = Math.Round(cr *= m.Rate, 4));
            return Math.Round(finalRate, 4);
        }
        public static double CalcDrawDown(IList<CalcModel> models)
        {
            var max = double.MinValue;
            foreach (var m in models)
            {
                max = max > m.CumRate ? max : m.CumRate;
                m.DrawDown = max > m.CumRate ? Math.Round(100 * (max - m.CumRate) / max, 2) : 0.0;
            }
            return models.Max(m => m.DrawDown);
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
