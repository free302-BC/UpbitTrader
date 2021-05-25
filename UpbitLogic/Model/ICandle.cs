using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit.Model
{
    /// <summary>
    /// Candle API의 리턴 모델 - CandleModel로 변환시
    /// </summary>
    public interface ICandle : IApiModel
    {
        #region ---- Public Properties ----

        public string CandleDateTimeKst { get; set; }
        public string CandleDateTimeUtc { get; set; }
        public long Timestamp { get; set; }
        public decimal OpeningPrice { get; set; }
        public decimal HighPrice { get; set; }
        public decimal LowPrice { get; set; }
        public decimal TradePrice { get; set; }
        
        public ApiId ApiId { get; }
        public CandleUnit CandleUnit { get; }

        #endregion


        #region ---- ICandle Type ~ ApiId Map ----

        public static ApiId GetApiId<C>() where C : ICandle
        {
            var type = typeof(C);
            if (!_typeApiDic.ContainsKey(type))
                throw new Exception($"{nameof(ICandle)}.{nameof(GetApiId)}(): Can't get ApiId of <{type.Name}>");
            return _typeApiDic[typeof(C)];
        }
        static Dictionary<Type, ApiId> _typeApiDic;

        #endregion


        #region ---- Allowed ApiId, CandleUnit for ICandle Modles ----

        public static void CheckParam(ApiId api)
        {
            if (!_apiNameDic.ContainsKey(api))
                throw new ArgumentException($"{nameof(ICandle)}: ApiId '{api}' not allowed.");
        }
        public static void CheckParam<C>(CandleUnit unit) where C : ICandle//CandleBase<C>
        {
            var api = GetApiId<C>();
            if (api == ApiId.CandleMinutes)
            {
                if (_units.Contains(unit)) return;
            }
            else
            {
                if (unit == CandleUnit.None) return;
            }
            throw new ArgumentException($"{nameof(ICandle)}: CandleUnit '{unit}' not allowed.");
        }
        public static string GetApiName(ApiId api, CandleUnit unit = CandleUnit.None)
        {
            CheckParam(api);
            if (unit == CandleUnit.None) return _apiNameDic[api];
            else return $"{_apiNameDic[api]}{(int)unit}";
        }
        static CandleUnit[] _units;
        static Dictionary<ApiId, string> _apiNameDic;

        #endregion


        static ICandle()
        {
            _apiNameDic = new()
            {
                { ApiId.CandleDays, "Day" },
                { ApiId.CandleMinutes, "Min" },
                { ApiId.CandleWeeks, "Week" },
                { ApiId.CandleMonth, "Month" },
            };

            _typeApiDic = new()
            {
                { typeof(CandleDay), ApiId.CandleDays },
                { typeof(CandleMinute), ApiId.CandleMinutes },

            };

            _units = Enum.GetValues<CandleUnit>().Where(x => x != CandleUnit.None)?.ToArray()!;
        }

    }//class
}
