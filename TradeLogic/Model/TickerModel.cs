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
        //입력
        string Market;
        DateTime TimeKST;
        decimal Opening, High, Low, Closing, Delta;
        TickerDir Dir;

        //계산용


        public TickerModel() => Market = "";

        public TickerModel(ITicker ticker)
        {
            Market = string.IsNullOrWhiteSpace(ticker.Market) ? ticker.Code : ticker.Market;
            Market = Market[^3..];

            //TimeKST = parse(ticker.TradeDateKst, ticker.TradeTimeKst);
            TimeKST = DateTimeOffset.FromUnixTimeMilliseconds(ticker.Timestamp).LocalDateTime;

            Opening = Math.Round(ticker.OpeningPrice / 10000.0m, 1);
            High = Math.Round(ticker.HighPrice / 10000.0m, 1);
            Low = Math.Round(ticker.LowPrice / 10000.0m, 1);
            Closing = Math.Round(ticker.TradePrice / 10000.0m, 1);
            Delta = Math.Round(ticker.SignedChangePrice / 10000.0m, 1);

            Dir = ticker.Change[0] switch
            {
                'E' => TickerDir.E,
                'R' => TickerDir.R,
                'F' => TickerDir.F,
                _ => throw new NotImplementedException()
            };
        }

        public override string ToString()
            => $"[{TimeKST:HH:mm:ss.fff}] {Opening,8:F1} {High,8:F1} {Low,8:F1} {Closing,8:F1} : {Delta,8:F1} {Dir} | {Market,8}";

        static readonly (string name, int wdith)[] _names =
        {
            (nameof(TimeKST), 14),
            (nameof(Opening), 8),
            (nameof(High), 8),
            (nameof(Low),  8),
            (nameof(Closing), 8),
            (nameof(Delta), 8),
            (nameof(Dir), 8),
            (nameof(Market), 8)
        };
        static TickerModel() => IViewModel.buildHeader(_names);
    }//class


    public static class _TickerModel
    {
        public static List<TickerModel> ToModels(this IEnumerable<ITicker> models)
           => models.Select(x => new TickerModel(x)).Reverse().ToList();
        public static TickerModel ToModel(this ITicker model) => new(model);
    }


}
