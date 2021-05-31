using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic.Model
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

        /// <summary>
        /// TODO: CandleUnit에 정의되지 않은 값일 경우(타 거래소)?
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static ApiId GetApiId(CandleUnit unit)
        {
            return _unitApiDic[unit];
        }

        public static string GetApiName(ApiId api, CandleUnit unit = CandleUnit.None)
        {
            if(api == ApiId.CandleMinutes) 
                return $"{_apiNameDic[api]}{(int)unit}";
            else 
                return _apiNameDic[api];
        }
        static Dictionary<ApiId, string> _apiNameDic;
        static Dictionary<CandleUnit, ApiId> _unitApiDic;

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
            _unitApiDic = new Dictionary<CandleUnit, ApiId>()
            {
                //{ CandleUnit.None, ApiId.None },                
                { CandleUnit.DAY, ApiId.CandleDays },
                { CandleUnit.WEEK, ApiId.CandleWeeks },
                { CandleUnit.MONTH, ApiId.CandleMonth },
            };
            var minutes = Enum.GetValues<CandleUnit>().Where(x => x >= CandleUnit.M1 && x < CandleUnit.DAY).ToArray();
            foreach (var m in minutes) _unitApiDic.Add(m, ApiId.CandleMinutes);

        }

    }//class
}
