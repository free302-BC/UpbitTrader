using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.Upbit.Model
{
    [DataContract]
    public class TradeTick : IApiModel
    {
        public TradeTick() => Market = TradeDateUtc = TradeTimeUtc = AskBid = "";

        /// <summary>
        /// 마켓 구분 코드
        /// </summary>
        /// <value>마켓 구분 코드</value>
        [DataMember(Name = "market", EmitDefaultValue = false)]
        public string Market { get; set; }

        /// <summary>
        /// 체결 일자 (UTC 기준)
        /// </summary>
        /// <value>체결 일자 (UTC 기준)</value>
        [DataMember(Name = "trade_date_utc", EmitDefaultValue = false)]
        public string TradeDateUtc { get; set; }

        /// <summary>
        /// 체결 시각 (UTC 기준)
        /// </summary>
        /// <value>체결 시각 (UTC 기준)</value>
        [DataMember(Name = "trade_time_utc", EmitDefaultValue = false)]
        public string TradeTimeUtc { get; set; }

        /// <summary>
        /// 체결 타임스탬프
        /// </summary>
        /// <value>체결 타임스탬프</value>
        [DataMember(Name = "timestamp", EmitDefaultValue = false)]
        public long Timestamp { get; set; }

        /// <summary>
        /// 체결 가격
        /// </summary>
        /// <value>체결 가격</value>
        [DataMember(Name = "trade_price", EmitDefaultValue = false)]
        public decimal TradePrice { get; set; }

        /// <summary>
        /// 체결량
        /// </summary>
        /// <value>체결량</value>
        [DataMember(Name = "trade_volume", EmitDefaultValue = false)]
        public decimal TradeVolume { get; set; }

        /// <summary>
        /// 전일 종가
        /// </summary>
        /// <value>전일 종가</value>
        [DataMember(Name = "prev_closing_price", EmitDefaultValue = false)]
        public decimal PrevPrice { get; set; }

        /// <summary>
        /// 변화량
        /// </summary>
        /// <value>변화량</value>
        [DataMember(Name = "change_price", EmitDefaultValue = false)]
        public decimal ChangePrice { get; set; }

        /// <summary>
        /// 매도/매수
        /// </summary>
        /// <value>매도/매수</value>
        [DataMember(Name = "ask_bid", EmitDefaultValue = false)]
        public string AskBid { get; set; }

        /// <summary>
        /// 체결 번호 (Unique)  &#x60;sequential_id&#x60; 필드는 체결의 유일성 판단을 위한 근거로 쓰일 수 있습니다. 하지만 체결의 순서를 보장하지는 못합니다. 
        /// </summary>
        /// <value>체결 번호 (Unique)  &#x60;sequential_id&#x60; 필드는 체결의 유일성 판단을 위한 근거로 쓰일 수 있습니다. 하지만 체결의 순서를 보장하지는 못합니다. </value>
        [DataMember(Name = "sequential_id", EmitDefaultValue = false)]
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
