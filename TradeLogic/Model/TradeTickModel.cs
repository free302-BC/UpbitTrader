using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic.Model
{
    public class TradeTickModel : IViewModel<TradeTickModel, ITradeTick>
    {
        //입력
        public DateTime TimeKST;
        public decimal UnitPrice, Volume, Price, Change;
        public long Serial;
        public string Dir;

        //계산용


        public TradeTickModel() { Dir = ""; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public TradeTickModel(ITradeTick tick) => setApiModel(tick);
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        void setApiModel(ITradeTick tick)
        {
            TimeKST = DateTimeOffset.FromUnixTimeMilliseconds(tick.Timestamp).LocalDateTime;
            UnitPrice = tick.TradePrice / 10000m;
            Volume = tick.TradeVolume;
            Price = UnitPrice * Volume;
            Change = tick.ChangePrice / 10000m;
            Dir = tick.AskBid[0] == 'B' ? "▲" : "▼";

            //SequentialId에서 miliseconds 000 제거, 시간단위 이상 제거
            // == 1시간 내에서 유효한 unique serial
            var msAtTheHour = new DateTimeOffset(TimeKST.Date).AddHours(TimeKST.Hour).ToUnixTimeMilliseconds();//
            Serial = tick.SequentialId / 1000 - msAtTheHour + (tick.SequentialId % 1000);//초당 1000개 
            //Serial = tick.SequentialId % 10000000L;
        }
        public override string ToString()
            => $"[{TimeKST:HHmmss.fff}] {Volume:F8} × {UnitPrice,6:F1} = {Price,7:F1}  {Dir,-6} | {Change,7:F1}";

        static TradeTickModel() => IViewModel.buildHeader(_names);
        static (string, int)[] _names =
        {
            (nameof(TimeKST), 12),
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
        public static List<TradeTickModel> ToModels(this IEnumerable<ITradeTick> models)
           => models.Select(x => new TradeTickModel(x)).Reverse().ToList();
        public static TradeTickModel ToModel(this ITradeTick model) => new TradeTickModel(model);
    }


}
