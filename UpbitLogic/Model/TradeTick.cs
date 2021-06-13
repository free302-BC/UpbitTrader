using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

#pragma warning disable CS8618

namespace Universe.Coin.Upbit.Model
{
    //[JsonConverter(typeof())]
    public class TradeTick : ITradeTick
    {
        //public TradeTick() => Market = TradeDateUtc = TradeTimeUtc = AskBid = Code = "";

        /// <summary>
        /// 마켓 구분 코드
        /// </summary>
        /// <value>마켓 구분 코드</value>
        [JsonPropertyName("market")]
        public string Market { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// 체결 일자 (UTC 기준)
        /// </summary>
        /// <value>체결 일자 (UTC 기준)</value>
        [JsonPropertyName("trade_date_utc")]
        public string TradeDateUtc { get; set; }

        /// <summary>
        /// 체결 시각 (UTC 기준)
        /// </summary>
        /// <value>체결 시각 (UTC 기준)</value>
        [JsonPropertyName("trade_time_utc")]
        public string TradeTimeUtc { get; set; }

        /// <summary>
        /// 체결 타임스탬프
        /// </summary>
        /// <value>체결 타임스탬프</value>
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        /// <summary>
        /// 체결 가격
        /// </summary>
        /// <value>체결 가격</value>
        [JsonPropertyName("trade_price")]
        public decimal TradePrice { get; set; }

        /// <summary>
        /// 체결량
        /// </summary>
        /// <value>체결량</value>
        [JsonPropertyName("trade_volume")]
        public decimal TradeVolume { get; set; }

        /// <summary>
        /// 전일 종가
        /// </summary>
        /// <value>전일 종가</value>
        [JsonPropertyName("prev_closing_price")]
        public decimal PrevPrice { get; set; }

        /// <summary>
        /// 변화량
        /// </summary>
        /// <value>변화량</value>
        [JsonPropertyName("change_price")]
        public decimal ChangePrice { get; set; }

        /// <summary>
        /// 매도/매수
        /// </summary>
        /// <value>매도/매수</value>
        [JsonPropertyName("ask_bid")]
        public string AskBid { get; set; }

        /// <summary>
        /// 체결 번호 (Unique)
        /// </summary>
        [JsonPropertyName("sequential_id")]
        public long SequentialId { get; set; }
        
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Market:       {Market}");
            sb.AppendLine($"TradeDateUtc: {TradeDateUtc}");
            sb.AppendLine($"TradeTimeUtc: {TradeTimeUtc}");
            sb.AppendLine($"Timestamp:    {Timestamp}");
            sb.AppendLine($"TradePrice:   {TradePrice}");
            sb.AppendLine($"TradeVolume:  {TradeVolume}");
            sb.AppendLine($"PrevPrice:    {PrevPrice}");
            sb.AppendLine($"ChangePrice:  {ChangePrice}");
            sb.AppendLine($"AskBid:       {AskBid}");
            sb.AppendLine($"SequentialId: {SequentialId}");
            return sb.ToString();
        }
    }//class
}

#pragma warning restore CS8618
