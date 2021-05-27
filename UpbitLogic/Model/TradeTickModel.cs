using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.Upbit.Model
{
    public class TradeTickModel : ViewModelBase<TradeTickModel, TradeTick>
    {
        public DateTime Time;
        public decimal UnitPrice, Volume, Price, Change;
        public long Serial;
        public string Dir;

        public TradeTickModel() { Dir = ""; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public TradeTickModel(TradeTick tick) => setApiModel(tick);
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected override TradeTickModel setApiModel(TradeTick tick)
        {
            Time = DateTimeOffset.FromUnixTimeMilliseconds(tick.Timestamp).LocalDateTime;
            UnitPrice = tick.TradePrice / 10000m;
            Volume = tick.TradeVolume;
            Price = UnitPrice * Volume;
            Change = tick.ChangePrice / 10000m;
            Dir = tick.AskBid[0] == 'B' ? "▲" : "▼";

            //SequentialId에서 mili seconds 000 제거, 시간단위 이상 제거
            var msAtTheHour = new DateTimeOffset(Time.Date).AddHours(Time.Hour).ToUnixTimeMilliseconds();//
            Serial = tick.SequentialId / 1000 - msAtTheHour + (tick.SequentialId % 1000);//초당 1000개 
            //Serial = tick.SequentialId % 10000000L;
            return this;
        }
        public override string ToString()
            => $"[{Time:HHmmss.fff}] {Volume:F8} × {UnitPrice,6:F1} = {Price,7:F1}  {Dir,-6} | {Change,7:F1}";

        static TradeTickModel() => IViewModel.buildHeader(_names);
        static (string, int)[] _names =
        {
            (nameof(Time), 12),
            (nameof(Volume), 10),
            ("UnPrice", 8),
            (nameof(Price), 9),
            (" Dir", -6),
            ("Δ", -7),
            //(nameof(Serial), 0),
        };

    }//class

    public static class _TradeTickModel
    {
        public static List<TradeTickModel> ToModels(this IEnumerable<TradeTick> models)
           => models.Select(x => TradeTickModel.ToModel(x)).Reverse().ToList();
        public static TradeTickModel ToModel(this TradeTick model)
           => TradeTickModel.ToModel(model);
    }


}
