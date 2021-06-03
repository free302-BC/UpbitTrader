using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic.Model
{
    public class M : ICalcModel
    {
        //입력
        public DateTime TimeKST;
        public decimal UnitPrice, Volume, Price, Change;
        public long Serial;
        public TradeTickDir Dir;

        //계산용
        public decimal MovingAvg { get; set; }
        public decimal MacdOsc { get; set; }
        public decimal Target { get; set; }
        public decimal Rate { get; set; }
        public decimal CumRate { get; set; }
        public decimal DrawDown { get; set; }

        public static readonly M Empty = new();

        public M() { }
        public M(ITradeTick tick) => setApiModel(tick);

        void setApiModel(ITradeTick tick)
        {
            TimeKST = DateTimeOffset.FromUnixTimeMilliseconds(tick.Timestamp).LocalDateTime;
            UnitPrice = tick.TradePrice / 10000m;
            Volume = tick.TradeVolume;
            Price = UnitPrice * Volume;
            Change = tick.ChangePrice / 10000m;
            Dir = tick.AskBid[0] == 'B' ? TradeTickDir.U : TradeTickDir.D;

            //SequentialId에서 miliseconds 000 제거, 시간단위 이상 제거
            // == 1시간 내에서 유효한 unique serial
            var msAtTheHour = new DateTimeOffset(TimeKST.Date).AddHours(TimeKST.Hour).ToUnixTimeMilliseconds();//
            Serial = tick.SequentialId / 1000 - msAtTheHour + (tick.SequentialId % 1000);//초당 1000개 
            //Serial = tick.SequentialId % 10000000L;
        }
        public override string ToString()
            => $"[{TimeKST:HH:mm:ss.fff}] {Volume:F8} × {UnitPrice,6:F1} = {Price,7:F1}  {Dir,1} {Change,5:F1}";

        static M() => IViewModel.buildHeader(_names);
        static (string, int)[] _names =
        {
            (nameof(TimeKST), 14),
            (nameof(Volume), 10),
            ("UnPrice", 8),
            (nameof(Price), 9),
            ("↕", 2),
            ("Δ", 5),
            //(nameof(Serial), 0),
        };

        
    }//class

    public static class _TradeTickModel
    {
        public static M[] ToModels(this IEnumerable<ITradeTick> models)
           => models.Select(x => new M(x)).Reverse().ToArray();
        public static M ToModel(this ITradeTick model) => new M(model);
    }


}
