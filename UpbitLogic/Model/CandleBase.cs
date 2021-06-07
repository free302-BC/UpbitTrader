using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.Upbit.Model
{
    [DataContract]
    public class CandleBase : ICandle// where C: CandleBase<C>
    {
        public CandleBase()
        {
            Market = CandleDateTimeUtc = CandleDateTimeKst = string.Empty;        
        }        
        public CandleBase(ApiId api, CandleUnit unit) : this()
        {
            //ICandle.CheckParam(unit);
            ApiId = api;
            CandleUnit = unit;
        }

        public ApiId ApiId { get; protected set; }
        public virtual CandleUnit CandleUnit { get; set; }

        /// <summary>
        /// 마켓명
        /// </summary>
        /// <value>마켓명</value>
        [JsonPropertyName("market")]
        public string Market { get; set; }

        /// <summary>
        /// 캔들 기준 시각 (UTC 기준)
        /// </summary>
        /// <value>캔들 기준 시각 (UTC 기준)</value>
        [JsonPropertyName("candle_date_time_utc")]
        public string CandleDateTimeUtc { get; set; }

        /// <summary>
        /// 캔들 기준 시각 (KST 기준)
        /// </summary>
        /// <value>캔들 기준 시각 (KST 기준)</value>
        [JsonPropertyName("candle_date_time_kst")]
        public string CandleDateTimeKst { get; set; }

        /// <summary>
        /// 시가
        /// </summary>
        /// <value>시가</value>
        [JsonPropertyName("opening_price")]
        public decimal OpeningPrice { get; set; }

        /// <summary>
        /// 고가
        /// </summary>
        /// <value>고가</value>
        [JsonPropertyName("high_price")]
        public decimal HighPrice { get; set; }

        /// <summary>
        /// 저가
        /// </summary>
        /// <value>저가</value>
        [JsonPropertyName("low_price")]
        public decimal LowPrice { get; set; }

        /// <summary>
        /// 종가
        /// </summary>
        /// <value>종가</value>
        [JsonPropertyName("trade_price")]
        public decimal TradePrice { get; set; }

        /// <summary>
        /// 해당 캔들에서 마지막 틱이 저장된 시각
        /// </summary>
        /// <value>해당 캔들에서 마지막 틱이 저장된 시각</value>
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        /// <summary>
        /// 누적 거래 금액
        /// </summary>
        /// <value>누적 거래 금액</value>
        [JsonPropertyName("candle_acc_trade_price")]
        public decimal CandleAccTradePrice { get; set; }

        /// <summary>
        /// 누적 거래량
        /// </summary>
        /// <value>누적 거래량</value>
        [JsonPropertyName("candle_acc_trade_volume")]
        public decimal CandleAccTradeVolume { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"ApiId:               {ApiId}");
            sb.AppendLine($"CandleUnit:          {CandleUnit}");
            sb.AppendLine($"Market:              {Market}");
            sb.AppendLine($"CandleDateTimeUtc:   {CandleDateTimeUtc}");
            sb.AppendLine($"CandleDateTimeKst:   {CandleDateTimeKst}");
            sb.AppendLine($"OpeningPrice:        {OpeningPrice}");
            sb.AppendLine($"HighPrice:           {HighPrice}");
            sb.AppendLine($"LowPrice:            {LowPrice}");
            sb.AppendLine($"TradePrice:          {TradePrice}");
            sb.AppendLine($"Timestamp:           {Timestamp}");
            sb.AppendLine($"CandleAccTradePrice: {CandleAccTradePrice}");
            sb.AppendLine($"CandleAccTradeVolume:{CandleAccTradeVolume}");
            return sb.ToString();
        }

    }//class
}
