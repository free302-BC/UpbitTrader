using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic.Model
{
    public class CandleModel : IViewModel<CandleModel, ICandle>
    {
        //입력
        public ApiId ApiId;
        public CandleUnit Unit;
        public DateTime TimeKST;
        public decimal Opening, High, Low, Closing, Delta;

        //계산용
        public decimal MovingAvg, MacdOsc, Target, Rate, CumRate, DrawDown;
        public const decimal FeeRate = 0.0005m * 2m;

        public static readonly CandleModel Empty = new() { Delta = 99999m };

        public CandleModel() { }
        public CandleModel(ICandle candle)
        {
            //SetApiModel(candle);
            ApiId = candle.ApiId;
            Unit = candle.CandleUnit;
            TimeKST = DateTime.Parse(candle.CandleDateTimeKst);
            Opening = Math.Round(candle.OpeningPrice / 10000.0m, 1);
            High = Math.Round(candle.HighPrice / 10000.0m, 1);
            Low = Math.Round(candle.LowPrice / 10000.0m, 1);
            Closing = Math.Round(candle.TradePrice / 10000.0m, 1);
            Delta = High - Low;
        }

        //public CandleModel SetApiModel(ICandle candle)
        //{
        //    ApiId = candle.ApiId;
        //    Unit = candle.CandleUnit;
        //    TimeKST = DateTime.Parse(candle.CandleDateTimeKst);
        //    Opening = Math.Round(candle.OpeningPrice / 10000.0m, 1);
        //    High = Math.Round(candle.HighPrice / 10000.0m, 1);
        //    Low = Math.Round(candle.LowPrice / 10000.0m, 1);
        //    Closing = Math.Round(candle.TradePrice / 10000.0m, 1);
        //    Delta = High - Low;
        //    return this;
        //}
        public override string ToString()
            => $"{TimeKST:yyMMdd.HHmm} {ICandle.GetApiName(ApiId, Unit),8} {Opening,8:F1} {MacdOsc,4:F2}"
            + $" {Target,8:F1} {High,8:F1} {Closing,8:F1} {Rate,8:F4} {CumRate,8:F4} {DrawDown,8:F2}";

        static readonly (string, int)[] _names =
        {
            (nameof(TimeKST), 11),
            (nameof(ApiId),    8),
            (nameof(MacdOsc),  4),
            (nameof(Opening),  8),
            (nameof(Target),   8),
            (nameof(High),     8),
            (nameof(Closing),  8),
            (nameof(Rate),     8),
            (nameof(CumRate),  8),
            (nameof(DrawDown), 8)
        };
        static CandleModel() => IViewModel.buildHeader(_names);

    }//class

    public static class _CandleModel
    {
        public static CandleModel[] ToModels(this IEnumerable<ICandle> models)
           => models.Select(x => new CandleModel(x)).Reverse().ToArray();
        public static CandleModel ToModel(this ICandle model) => new CandleModel(model);

    }   


}
