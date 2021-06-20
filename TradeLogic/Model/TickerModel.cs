using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic.Model
{
    public class TickerModel : ICalcModel
    {
        //입력
        public string Market;
        public DateTime TimeKST;
        public decimal Opening, High, Low, Closing, Delta;
        public TickerDir Dir;

        //계산용
        public decimal Value { get; set; }
        public long Timestamp { get; set; }
        public decimal MovingAvg { get; set; }
        public decimal Macd { get; set; }
        public decimal MacdOsc { get; set; }
        public decimal Target { get; set; }
        public bool TradeDone { get; set; }
        public TimingSignal Signal { get; set; }
        public decimal Rate { get; set; } = 1.0m;
        public decimal CumRate { get; set; } = 1.0m;
        public decimal DrawDown { get; set; }

        public static readonly TickerModel Empty = new();

        public TickerModel() => Market = "";

        public TickerModel(ITicker ticker)
        {
            Market = string.IsNullOrWhiteSpace(ticker.Market) ? ticker.Code : ticker.Market;
            Market = Market[^3..];

            Timestamp = ticker.Timestamp;
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

            Value = Closing;
        }

        public override string ToString()
            => $"[{TimeKST:HH:mm:ss.fff}] {Opening,8:F1} {High,8:F1} {Low,8:F1} {Closing,8:F1} : {Delta,8:F1} {Dir} | {Market,8}";

        public string ToCalcString()
            => $"[{TimeKST:HH:mm:ss.fff}]{Closing,8:F1}{MovingAvg,8:F1}{Macd,8:F2}{MacdOsc,8:F2}{Signal,5} |{Rate,8:F4}{CumRate,8:F4}";
        public string CalcHeader => $"[TimeKST]\tPrice\tDir\tMacd\tSignal\tRate\tCumRate";

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
