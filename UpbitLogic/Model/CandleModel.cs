using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit.Model
{
    public class CandleModel : ViewModelBase<CandleModel, ICandle>
    {
        public DateTime TimeKST;
        public decimal Opening;
        public decimal High;
        public decimal Low;
        public decimal Closing;
        public decimal Delta;

        public decimal Target;
        public decimal Rate;
        public decimal CumRate;
        public decimal DrawDown;
        const decimal _feeRate = 0.0005m * 2m;
        static readonly CandleModel _default = new();

        public CandleModel() { }
        public CandleModel(ICandle candle) => setApiModel(candle);
        protected override CandleModel setApiModel(ICandle candle)
        {
            TimeKST = DateTime.Parse(candle.CandleDateTimeKst);
            Opening = Math.Round(candle.OpeningPrice / 10000.0m, 1);
            High = Math.Round(candle.HighPrice / 10000.0m, 1);
            Low = Math.Round(candle.LowPrice / 10000.0m, 1);
            Closing = Math.Round(candle.TradePrice / 10000.0m, 1);
            Delta = High - Low;
            return this;
        }


        void calcRate(CandleModel prev, decimal k)
        {
            Target = Math.Round(Opening + prev.Delta * k, 2);
            Rate = (High > Target) ? Math.Round(Closing / Target - _feeRate, 4) : 1.0m;
        }
        public override string ToString()
            => $"{TimeKST:yyMMdd.HHmm} {Opening,8:F1} {Target,8:F1} {High,8:F1} {Closing,8:F1} : {Rate,8:F4} {CumRate,8:F4} {DrawDown,8:F2}";

        
        public static void CalcRate(IList<CandleModel> models, decimal k)
        {
            models.Insert(0, _default);
            for (int i = 1; i < models.Count; i++) models[i].calcRate(models[i - 1], k);
            models.RemoveAt(0);
        }
        public static decimal CalcCumRate(IList<CandleModel> models)
        {
            var finalRate = models.Aggregate(1.0m, (cr, m) => m.CumRate = Math.Round(cr *= m.Rate, 4));
            return Math.Round(finalRate, 4);
        }
        public static decimal CalcDrawDown(IList<CandleModel> models)
        {
            var max = decimal.MinValue;
            foreach (var m in models)
            {
                max = max > m.CumRate ? max : m.CumRate;
                m.DrawDown = max > m.CumRate ? Math.Round(100 * (max - m.CumRate) / max, 2) : 0m;
            }
            return models.Max(m => m.DrawDown);
        }
       
        static readonly (string, int)[] _names =
        {
            (nameof(TimeKST), 11), 
            (nameof(Opening),  8),
            (nameof(Target),   8),
            (nameof(High),     8),
            (nameof(Closing),  8),
            (nameof(Rate),     10),
            (nameof(CumRate),  8),
            (nameof(DrawDown), 8)
        };
        static CandleModel() => IViewModel.buildHeader(_names);
        
    }//class

    public static class _CandleModel
    {
        public static List<CandleModel> ToModels(this IEnumerable<ICandle> models)
           => models.Select(x => CandleModel.ToModel(x)).Reverse().ToList();
        public static CandleModel ToModel(this ICandle model)
           => CandleModel.ToModel(model);
    }

}
