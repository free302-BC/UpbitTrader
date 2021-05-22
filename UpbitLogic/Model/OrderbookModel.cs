using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit.Model
{
    public class OrderbookModel : ViewModelBase<OrderbookModel, Orderbook>
    {
        public DateTime time;
        public decimal askUP, bidUP, deltaUP; //unit price (price per 1 coin)
        public decimal askA, bidA;//amount (number of coins)
        public decimal askP, bidP;
        public OrderbookModel() { }
        public OrderbookModel(Orderbook book) => setApiModel(book);
        protected override OrderbookModel setApiModel(Orderbook book)
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
            return this;
        }
        public override string ToString()
            => $"[{time:yyMMdd.HHmmss.fff}] {askA:F8} × {askUP,6:F1} = {askP,7:F1}  | {deltaUP,3:F1} | {bidP,7:F1} = {bidUP,6:F1} × {bidA:F8}";

        public static string Print(IEnumerable<OrderbookModel> models)
        {
            var sb = new StringBuilder();
            sb.AppendLine(_header);
            foreach (var m in models) sb.AppendLine(m.ToString());
            return sb.ToString();
        }

        static readonly string _header;
        static OrderbookModel()
        {
            StringBuilder sb = new();
            foreach (var h in _names) sb.Append($"{{{h.name},{h.wdith}:{h.fmt}}} ");
            _header = sb.ToString();
        }

        
        static readonly (string name, int wdith, string fmt)[] _names =
        {
            //$"{askA:F8} × {askUP,6:F1} = {askP,7:F1}  | {deltaUP,3:F1} | {bidP,7:F1} = {bidUP,6:F1} × {bidA:F8}";
            (nameof(time), 17, ""),
            (nameof(askA), 10, "F8"),
            (nameof(askUP), 6, "F1"),
            (nameof(askP),  7, "F1"),
            (nameof(deltaUP), 3, "F1"),
            (nameof(bidP), 7, "F1"),
            (nameof(bidUP), 6, "F1"),
            (nameof(bidA), 10, "F8")
        };

    }//class

    public static class _OrderbookModel
    {
        public static List<OrderbookModel> ToModels(this IEnumerable<Orderbook> models)
           => models.Select(x => OrderbookModel.ToModel(x)).Reverse().ToList();
        public static OrderbookModel ToModel(this Orderbook model)
           => OrderbookModel.ToModel(model);
    }


}
