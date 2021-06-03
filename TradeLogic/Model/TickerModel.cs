using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic.Model
{
    public class TickerModel : IViewModel
    {
        string Market, TimeKST, Change;
        decimal Opening, High, Low, Closing, Delta;
        public TickerModel() => Market = TimeKST = Change = "";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public TickerModel(ITicker ticker) => setApiModel(ticker);
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        void setApiModel(ITicker ticker)
        {
            Market = ticker.Market;
            TimeKST = $"{ticker.TradeDateKst}.{ticker.TradeTimeKst}";
            Opening = Math.Round(ticker.OpeningPrice / 10000.0m, 1);
            High = Math.Round(ticker.HighPrice / 10000.0m, 1);
            Low = Math.Round(ticker.LowPrice / 10000.0m, 1);
            Closing = Math.Round(ticker.TradePrice / 10000.0m, 1);
            Delta = Math.Round(ticker.SignedChangePrice / 10000.0m, 1);
            Change = ticker.Change == "EVEN" ? "〓" : $"{(ticker.Change == "RISE" ? "▲" : "▼")}";
        }

        public override string ToString()
            => $"{Market,8} {TimeKST,15} {Opening,8:F1} {High,8:F1} {Low,8:F1} {Closing,8:F1} : {Delta,8:F1} {Change}";

        static readonly (string name, int wdith)[] _names =
        {
            (nameof(Market), 8),
            (nameof(TimeKST), 15),
            (nameof(Opening), 8),
            (nameof(High), 8),
            (nameof(Low),  8),
            (nameof(Closing), 8),
            (nameof(Delta), 8),
            (nameof(Change), 8)
        };
        static TickerModel() => IViewModel.buildHeader(_names);
    }//class


    public static class _TickerModel
    {
        public static List<TickerModel> ToModels(this IEnumerable<ITicker> models)
           => models.Select(x => new TickerModel(x)).Reverse().ToList();
        public static TickerModel ToModel(this ITicker model) => new TickerModel(model);
    }


}
