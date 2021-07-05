using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic.Model
{
    public class OrderbookModel : ICalcModel//IViewModel
    {
        //입력
        public string Market;
        public DateTime TimeKST;
        public decimal AskUnitPrice, BidUnitPrice, DeltaUnitPrice; //unit price (price per 1 coin)
        public decimal AskAmount, BidAmount;//amount (number of coins)
        public decimal AskPrice, BidPrice;

        //계산용
        public decimal Value { get; set; }
        public long Timestamp { get; set; }
        public decimal MovingAvg { get; set; }
        public decimal Macd { get; set; }
        public decimal MacdOsc { get; set; }

        public int NumAsks { get; set; }
        public int NumBids { get; set; }
        public decimal ABR { get; set; }

        public decimal Target { get; set; }
        public bool TradeDone { get; set; }
        public TimingSignal Signal { get; set; }
        public decimal Rate { get; set; } = 1.0m;
        public decimal CumRate { get; set; } = 1.0m;
        public decimal DrawDown { get; set; }

        public static readonly OrderbookModel Empty = new();


#pragma warning disable CS8618
        public OrderbookModel() { }
#pragma warning restore CS8618

        public OrderbookModel(IOrderbook book)
        {
            Market = string.IsNullOrWhiteSpace(book.Market) ? book.Code : book.Market;
            Market = Market[^3..];

            TimeKST = DateTimeOffset.FromUnixTimeMilliseconds(book.Timestamp).LocalDateTime;

            var order = book.OrderbookUnits.First();
            AskAmount = order.AskSize;
            BidAmount = order.BidSize;

            AskUnitPrice = (order.AskPrice / 10000.0m);
            BidUnitPrice = (order.BidPrice / 10000.0m);
            DeltaUnitPrice = AskUnitPrice - BidUnitPrice;

            AskPrice = AskAmount * AskUnitPrice;
            BidPrice = BidAmount * BidUnitPrice;

            //
            Value = AskUnitPrice;
        }
        
        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is OrderbookModel)) return false;
            var o = (OrderbookModel)obj;
            return AskUnitPrice == o.AskUnitPrice && AskAmount == o.AskAmount && BidPrice == o.BidPrice && BidAmount == o.BidAmount;
        }
        public static bool operator ==(OrderbookModel? a, OrderbookModel? b) => a?.Equals(b) ?? false;
        public static bool operator !=(OrderbookModel? a, OrderbookModel? b) => !(a?.Equals(b) ?? false);
        public override int GetHashCode() => Utility.HashCode.Of(AskUnitPrice).And(AskAmount).And(BidUnitPrice).And(BidAmount);

        public override string ToString()
            => $"[{TimeKST:HH:mm:ss.fff}] {AskAmount:F8} × {AskUnitPrice,6:F1} = {AskPrice,7:F1}  | {DeltaUnitPrice,6:F1} | {BidPrice,7:F1} = {BidUnitPrice,6:F1} × {BidAmount:F8} {Market}";

        public string ToCalcString()
            => $"[{TimeKST:HH:mm:ss.fff}]{Value,8:F1}{DeltaUnitPrice,6:F1}{MacdOsc,6:F2}{Signal,5}{Rate,8:F5}{CumRate,8:F5}";
        public string CalcHeader => $"[{"TimeKST",12}]{"Price",8}{"Δ",6}{"Macd",6}{"Sig",5}{"Rate",8}{"CumRate",8}";

        static OrderbookModel() => IViewModel.buildHeader(_names);
        static readonly (string name, int wdith)[] _names =
        {
            (nameof(TimeKST),  12),
            (nameof(AskAmount),  10),
            (nameof(AskUnitPrice), 6),
            (nameof(AskPrice),  7),
            (nameof(DeltaUnitPrice), 4),
            (nameof(BidPrice), 7),
            (nameof(BidUnitPrice), 7),
            (nameof(BidAmount), 10)
        };

    }//class


    public static class _OrderbookModel
    {
        public static List<OrderbookModel> ToModels(this IEnumerable<IOrderbook> models)
           => models.Select(x => new OrderbookModel(x)).Reverse().ToList();
        public static OrderbookModel ToModel(this IOrderbook model) => new(model);
    }



}
