using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic.Model
{
    public class CandleModel : ViewModelBase<CandleModel, ICandle>
    {
        //입력
        public ApiId ApiId;
        public CandleUnit Unit;
        public DateTime TimeKST;
        public decimal Opening, High, Low, Closing, Delta;

        //계산용
        public decimal Target, Rate, CumRate, DrawDown;
        const decimal _feeRate = 0.0005m * 2m;
        static readonly CandleModel _empty = new() { Delta = 99999m };

        public CandleModel() { }
        public CandleModel(ICandle candle) => setApiModel(candle);

        protected override CandleModel setApiModel(ICandle candle)
        {
            ApiId = candle.ApiId;
            Unit = candle.CandleUnit;
            TimeKST = DateTime.Parse(candle.CandleDateTimeKst);
            Opening = Math.Round(candle.OpeningPrice / 10000.0m, 1);
            High = Math.Round(candle.HighPrice / 10000.0m, 1);
            Low = Math.Round(candle.LowPrice / 10000.0m, 1);
            Closing = Math.Round(candle.TradePrice / 10000.0m, 1);
            Delta = High - Low;
            return this;
        }

        void calcRate_OverDelta(CandleModel prev, decimal k)
        {
            Target = Math.Round(Opening + prev.Delta * k, 2);
            Rate = (High > Target) ? Math.Round(Closing / Target - _feeRate, 4) : 1.0m;
        }
        void calcRate_OverDeltaMA(CandleModel prev, decimal k, decimal ma)
        {
            Target = Math.Round(Opening + prev.Delta * k, 2);
            Rate = (High > Target && Opening >= ma) ? Math.Round(Closing / Target - _feeRate, 4) : 1.0m;
        }
        void calcRate_StopLoss(CandleModel prev, decimal k)
        {
            Target = Math.Round(Opening + prev.Delta * k, 2);

            var sellPrice = Target * 0.99m > Low ? Math.Max(Target * 0.985m, Low) : Closing;
            //하락후 회복시 미반영

            Rate = (High > Target) ? Math.Round(sellPrice / Target - _feeRate, 4) : 1.0m;
        }

        public override string ToString()
            => $"{TimeKST:yyMMdd.HHmm} {ICandle.GetApiName(ApiId, Unit),8} {Opening,8:F1} {Target,8:F1} {High,8:F1} {Closing,8:F1} {Rate,8:F4} {CumRate,8:F4} {DrawDown,8:F2}";


        public static void CalcRate(IList<CandleModel> models, decimal k, bool stopLoss = false)
        {
            //models.Insert(0, _empty);

            if (stopLoss)
            {
                models[0].calcRate_StopLoss(_empty, k);
                for (int i = 1; i < models.Count; i++)
                    models[i].calcRate_StopLoss(models[i - 1], k);
            }
            else
            {
                models[0].calcRate_OverDelta(_empty, k);
                for (int i = 1; i < models.Count; i++)
                    models[i].calcRate_OverDelta(models[i - 1], k);
            }

            //models.RemoveAt(0);
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
            (nameof(ApiId), 8),
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

        public static CandleModel[] ToModelArray(this IEnumerable<ICandle> models)
           => models.Select(x => CandleModel.ToModel(x)).Reverse().ToArray();
    }

}
