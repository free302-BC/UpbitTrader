using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit.Model
{
    [DataContract]
    public class CandleDay : ICandle
    {
        public CandleDay() => Market = CandleDateTimeUtc = CandleDateTimeKst = string.Empty;

        /// <summary>
        /// 마켓명
        /// </summary>
        /// <value>마켓명</value>
        [DataMember(Name = "market", EmitDefaultValue = false)]
        public string Market { get; set; }

        /// <summary>
        /// 캔들 기준 시각 (UTC 기준)
        /// </summary>
        /// <value>캔들 기준 시각 (UTC 기준)</value>
        [DataMember(Name = "candle_date_time_utc", EmitDefaultValue = false)]
        public string CandleDateTimeUtc { get; set; }

        /// <summary>
        /// 캔들 기준 시각 (KST 기준)
        /// </summary>
        /// <value>캔들 기준 시각 (KST 기준)</value>
        [DataMember(Name = "candle_date_time_kst", EmitDefaultValue = false)]
        public string CandleDateTimeKst { get; set; }

        /// <summary>
        /// 시가
        /// </summary>
        /// <value>시가</value>
        [DataMember(Name = "opening_price", EmitDefaultValue = false)]
        public decimal OpeningPrice { get; set; }

        /// <summary>
        /// 고가
        /// </summary>
        /// <value>고가</value>
        [DataMember(Name = "high_price", EmitDefaultValue = false)]
        public decimal HighPrice { get; set; }

        /// <summary>
        /// 저가
        /// </summary>
        /// <value>저가</value>
        [DataMember(Name = "low_price", EmitDefaultValue = false)]
        public decimal LowPrice { get; set; }

        /// <summary>
        /// 종가
        /// </summary>
        /// <value>종가</value>
        [DataMember(Name = "trade_price", EmitDefaultValue = false)]
        public decimal TradePrice { get; set; }

        /// <summary>
        /// 해당 캔들에서 마지막 틱이 저장된 시각
        /// </summary>
        /// <value>해당 캔들에서 마지막 틱이 저장된 시각</value>
        [DataMember(Name = "timestamp", EmitDefaultValue = false)]
        public long Timestamp { get; set; }

        /// <summary>
        /// 누적 거래 금액
        /// </summary>
        /// <value>누적 거래 금액</value>
        [DataMember(Name = "candle_acc_trade_price", EmitDefaultValue = false)]
        public decimal CandleAccTradePrice { get; set; }

        /// <summary>
        /// 누적 거래량
        /// </summary>
        /// <value>누적 거래량</value>
        [DataMember(Name = "candle_acc_trade_volume", EmitDefaultValue = false)]
        public decimal CandleAccTradeVolume { get; set; }

        /// <summary>
        /// 전일 종가 (UTC 0시 기준)
        /// </summary>
        /// <value>전일 종가 (UTC 0시 기준)</value>
        [DataMember(Name = "prev_closing_price", EmitDefaultValue = false)]
        public decimal PrevClosingPrice { get; set; }

        /// <summary>
        /// 전일 종가 대비 변화 금액
        /// </summary>
        /// <value>전일 종가 대비 변화 금액</value>
        [DataMember(Name = "change_price", EmitDefaultValue = false)]
        public decimal ChangePrice { get; set; }

        /// <summary>
        /// 전일 종가 대비 변화량
        /// </summary>
        /// <value>전일 종가 대비 변화량</value>
        [DataMember(Name = "change_rate", EmitDefaultValue = false)]
        public decimal ChangeRate { get; set; }

        /// <summary>
        /// 종가 환산 화폐 단위로 환산된 가격 (요청에 convertingPriceUnit 파라미터 없을 시 해당 필드 포함되지 않음.) 
        /// </summary>
        /// <value>종가 환산 화폐 단위로 환산된 가격 (요청에 convertingPriceUnit 파라미터 없을 시 해당 필드 포함되지 않음.) </value>
        [DataMember(Name = "converted_trade_price", EmitDefaultValue = false)]
        public decimal ConvertedTradePrice { get; set; }

        const int w = 20;

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Market:              {Market,w}");
            sb.AppendLine($"CandleDateTimeUtc:   {CandleDateTimeUtc,w}");
            sb.AppendLine($"CandleDateTimeKst:   {CandleDateTimeKst,w}");
            sb.AppendLine($"OpeningPrice:        {OpeningPrice,w}");
            sb.AppendLine($"HighPrice:           {HighPrice,w}");
            sb.AppendLine($"LowPrice:            {LowPrice,w}");
            sb.AppendLine($"TradePrice:          {TradePrice,w}");
            sb.AppendLine($"Timestamp:           {Timestamp,w}");
            sb.AppendLine($"CandleAccTradePrice: {CandleAccTradePrice,w}");
            sb.AppendLine($"CandleAccTradeVolume:{CandleAccTradeVolume,w}");
            sb.AppendLine($"PrevClosingPrice:    {PrevClosingPrice,w}");
            sb.AppendLine($"ChangePrice:         {ChangePrice,w}");
            sb.AppendLine($"ChangeRate:          {ChangeRate,w}");
            sb.AppendLine($"ConvertedTradePrice: {ConvertedTradePrice,w}");
            return sb.ToString();
        }

    }//class
}
