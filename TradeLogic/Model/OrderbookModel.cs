using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic.Model
{
    public class OrderbookModel : IViewModel<OrderbookModel, IOrderbook>
    {
        public DateTime time;
        public decimal askUP, bidUP, deltaUP; //unit price (price per 1 coin)
        public decimal askA, bidA;//amount (number of coins)
        public decimal askP, bidP;
        public OrderbookModel() { }
        public OrderbookModel(IOrderbook book)
        {
            setApiModel(book);
        }

        void setApiModel(IOrderbook book)
        {
            var order = book.OrderbookUnits[0];
            time = DateTimeOffset.FromUnixTimeMilliseconds(book.Timestamp).LocalDateTime;

            askA = order.AskSize;
            bidA = order.BidSize;

            askUP = (order.AskPrice / 10000.0m);
            bidUP = (order.BidPrice / 10000.0m);
            deltaUP = askUP - bidUP;

            askP = askA * askUP;
            bidP = bidA * bidUP;
        }
        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is OrderbookModel)) return false;
            var o = (OrderbookModel)obj;
            return askUP == o.askUP && askA == o.askA && bidP == o.bidP && bidA == o.bidA;
        }
        public static bool operator ==(OrderbookModel? a, OrderbookModel? b) => a?.Equals(b) ?? false;
        public static bool operator !=(OrderbookModel? a, OrderbookModel? b) => !(a?.Equals(b) ?? false);
        public override int GetHashCode() => Utility.HashCode.Of(askUP).And(askA).And(bidUP).And(bidA);

        public override string ToString()
            => $"[{time:HHmmss.fff}] {askA:F8} × {askUP,6:F1} = {askP,7:F1}  | {deltaUP,4:F1} | {bidP,7:F1} = {bidUP,6:F1} × {bidA:F8}";
        static OrderbookModel() => IViewModel.buildHeader(_names);
        static readonly (string name, int wdith)[] _names =
        {
            (nameof(time),  10),
            (nameof(askA),  10),
            (nameof(askUP), 6),
            (nameof(askP),  7),
            (nameof(deltaUP), 3),
            (nameof(bidP), 7),
            (nameof(bidUP), 6),
            (nameof(bidA), 10)
        };

    }//class


    public static class _OrderbookModel
    {
        public static List<OrderbookModel> ToModels(this IEnumerable<IOrderbook> models)
           => models.Select(x => new OrderbookModel(x)).Reverse().ToList();
        public static OrderbookModel ToModel(this IOrderbook model) => new OrderbookModel(model);
    }



}
