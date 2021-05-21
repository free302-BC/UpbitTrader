using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit.Model
{
    public class TickerModel
    {
        string TimeKST;
        decimal Opening;
        decimal High;
        decimal Low;
        decimal Closing;
        decimal Delta;
        string Change;

        public TickerModel()
        {
            TimeKST = Change = "";
        }

        public TickerModel(Ticker ticker)
        {
            TimeKST = $"{ticker.TradeDateKst}.{ticker.TradeTimeKst}";
            Opening = Math.Round(ticker.OpeningPrice / 10000.0m, 1);
            High = Math.Round(ticker.HighPrice / 10000.0m, 1);
            Low = Math.Round(ticker.LowPrice / 10000.0m, 1);
            Closing = Math.Round(ticker.TradePrice / 10000.0m, 1);
            Delta = Math.Round(ticker.SignedChangePrice / 10000.0m, 1);
            Change = ticker.Change=="EVEN" ? "〓":$"{(ticker.Change == "RISE"? "▲" : "▽")}";
        }
        public override string ToString()
            => $"{TimeKST,8} {Opening,8:F1} {High,8:F1} {Low,8:F1} {Closing,8:F1} : {Delta,8:F1} {Change}";

        public static string Print(IEnumerable<TickerModel> models)
        {
            var sb = new StringBuilder();
            sb.AppendLine(_header);
            foreach (var m in models) sb.AppendLine(m.ToString());
            return sb.ToString();
        }

        static readonly string _header;
        static TickerModel()
        {
            StringBuilder sb = new();
            foreach (var h in _names) sb.Append($"{{{h.name},{h.wdith}}} ");
            _header = sb.ToString();
        }
        static readonly (string name, int wdith)[] _names =
        {
            (nameof(TimeKST), 8), 
            (nameof(Opening), 8), 
            (nameof(High), 8), 
            (nameof(Low),  8),
            (nameof(Closing), 8),
            (nameof(Delta), 8),
            (nameof(Change), 8)
        };


    }//class
}
