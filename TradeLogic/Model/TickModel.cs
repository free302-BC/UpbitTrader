using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic.Model
{
    public class TickModel : ICalcModel
    {
        //입력
        public string Market;
        public DateTime TimeKST;
        public decimal UnitPrice, Volume, Price, Change;
        public long Serial;
        public TradeTickDir Dir;

        //계산용
        public decimal Value { get; set; }
        public long Timestamp { get; set; }
        public decimal MovingAvg { get; set; }
        public decimal Macd { get; set; }
        public decimal MacdOsc { get; set; }

        public int NumAsks { get; set; }
        public int NumBids { get; set; }
        public decimal ABR { get; set; } = 1m;

        public decimal Target { get; set; }
        public decimal BuyPrice { get; set; }
        public bool TradeDone { get; set; }
        public TimingSignal Signal { get; set; }
        public decimal Rate { get; set; } = 1m;
        public decimal CumRate { get; set; } = 1m;
        public decimal DrawDown { get; set; }

        public static readonly TickModel Empty = new();

#pragma warning disable CS8618
        public TickModel() { }
        public TickModel(ITradeTick tick)
        {
            Market = string.IsNullOrWhiteSpace(tick.Market) ? tick.Code : tick.Market;
            Market = Market[^3..];

            Timestamp = tick.Timestamp;
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

            Value = UnitPrice;
        }
#pragma warning restore CS8618

        public override string ToString()
            => $"[{TimeKST:HH:mm:ss.fff}] {Volume:F8} × {UnitPrice,6:F1} = {Price,7:F1}  {Dir,1} {Change,6:F1} | {Market}";

        public string ToCalcString()
            => $"[{TimeKST:HH:mm:ss.fff}]{Dir,2}{Value,8:F1}{MacdOsc,6:F2}{ABR,6:F3}({NumAsks}/{NumBids}){Signal,5}{Rate,8:F5}{CumRate,8:F5}";
        public string CalcHeader => $"[{"TimeKST",12}]{"↕",2}{"Price",8}{"Macd",6}{"ABR",6}({"nA"}/{"nB"}){"Sig",5}{"Rate",8}{"CumRate",8}";

        static TickModel() => IViewModel.buildHeader(_names);
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
        public static TickModel[] ToModels(this IEnumerable<ITradeTick> models)
           => models.Select(x => new TickModel(x)).Reverse().ToArray();
        public static TickModel ToModel(this ITradeTick model) => new(model);
    }


}
