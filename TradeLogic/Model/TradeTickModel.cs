using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic.Model
{
    public class TradeTickModel : ICalcModel
    {
        //입력
        public string Market;
        public DateTime TimeKST;
        public decimal UnitPrice, Volume, Price, Change;
        public long Serial;
        public TradeTickDir Dir;

        //계산용
        public decimal MovingAvg { get; set; }
        public decimal MacdOsc { get; set; }
        public decimal Target { get; set; }
        public bool TradeDone { get; set; }
        public TimingSignal Signal { get; set; }
        public decimal Rate { get; set; } = 1.0m;
        public decimal CumRate { get; set; } = 1.0m;
        public decimal DrawDown { get; set; }

        public const decimal FeeRate = 0.0005m;
        public static readonly TradeTickModel Empty = new();

#pragma warning disable CS8618
        public TradeTickModel() { }
        public TradeTickModel(ITradeTick tick)
        {
            Market = string.IsNullOrWhiteSpace(tick.Market) ? tick.Code : tick.Market;
            Market = Market[^3..];

            TimeKST = DateTimeOffset.FromUnixTimeMilliseconds(tick.Timestamp).LocalDateTime;
            UnitPrice = tick.TradePrice / 10000m;
            Volume = tick.TradeVolume;
            Price = UnitPrice * Volume;
            Change = tick.ChangePrice / 10000m;
            Dir = tick.AskBid[0] == 'B' ? TradeTickDir.B : TradeTickDir.A;

            //SequentialId에서 시간단위 이상 제거
            // == 1시간 내에서 유효한 unique serial
            var msAtTheHour = new DateTimeOffset(TimeKST.Date).AddHours(TimeKST.Hour).ToUnixTimeMilliseconds();//
            Serial = tick.SequentialId / 1000 - msAtTheHour + (tick.SequentialId % 1000);//초당 1000개 
            //Serial = (tick.SequentialId / 1000 - msAtTheHour) * 1000 + (tick.SequentialId % 1000);//초당 1000개 
            //Serial = tick.SequentialId % 100000000;
        }
#pragma warning restore CS8618

        public override string ToString()
            => $"[{TimeKST:HH:mm:ss.fff}] {Volume:F8} × {UnitPrice,6:F1} = {Price,7:F1}  {Dir,1} {Change,6:F1} | {Market}";

        public string ToCalcString()
            => $"[{TimeKST:HH:mm:ss.fff}]\t{UnitPrice,6:F1}\t{Dir,3}\t{MacdOsc,7:F2}\t{Signal,7}\t{Rate,8:F4}\t{CumRate,8:F4}";
        public static string CalcHeader
            = $"[TimeKST]\tUnitPrice\tDir\tMacdOsc\tSignal\tRate\tCumRate";

        static TradeTickModel() => IViewModel.buildHeader(_names);
        static (string, int)[] _names =
        {
            (nameof(TimeKST), 14),
            (nameof(Volume), 10),
            ("UnPrice", 8),
            (nameof(Price), 9),
            ("↕", 2),
            ("Δ", 6),
            (nameof(Serial), 0),
        };
        
    }//class

    public static class _TradeTickModel
    {
        public static TradeTickModel[] ToModels(this IEnumerable<ITradeTick> models)
           => models.Select(x => new TradeTickModel(x)).Reverse().ToArray();
        public static TradeTickModel ToModel(this ITradeTick model) => new(model);
    }


}
