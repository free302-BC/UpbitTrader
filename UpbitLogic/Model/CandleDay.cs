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
    public class CandleDay : IEquatable<CandleDay>, IValidatableObject, ICandle
    {
        static readonly string[] _names =
        {
            "Market","CandleDateTimeUtc","CandleDateTimeKst","OpeningPrice","HighPrice","LowPrice",
            "TradePrice","Timestamp","CandleAccTradePrice","CandleAccTradeVolume", "PrevClosingPrice",
            "ChangePrice","ChangeRate","ConvertedTradePrice"
        };
        public static string PrintNames(int w)
        {
            var fmt = string.Format($"{{ 0,-{w} }}");
            var sb = new StringBuilder();
            foreach (var n in _names) sb.AppendFormat(fmt, n);
            return sb.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CandleDay" /> class.
        /// </summary>
        /// <param name="market">마켓명.</param>
        /// <param name="candleDateTimeUtc">캔들 기준 시각 (UTC 기준).</param>
        /// <param name="candleDateTimeKst">캔들 기준 시각 (KST 기준).</param>
        /// <param name="openingPrice">시가.</param>
        /// <param name="highPrice">고가.</param>
        /// <param name="lowPrice">저가.</param>
        /// <param name="tradePrice">종가.</param>
        /// <param name="timestamp">해당 캔들에서 마지막 틱이 저장된 시각.</param>
        /// <param name="candleAccTradePrice">누적 거래 금액.</param>
        /// <param name="candleAccTradeVolume">누적 거래량.</param>
        /// <param name="prevClosingPrice">전일 종가 (UTC 0시 기준).</param>
        /// <param name="changePrice">전일 종가 대비 변화 금액.</param>
        /// <param name="changeRate">전일 종가 대비 변화량.</param>
        /// <param name="convertedTradePrice">종가 환산 화폐 단위로 환산된 가격 (요청에 convertingPriceUnit 파라미터 없을 시 해당 필드 포함되지 않음.) .</param>
        //public CandleDay(string market = "", string candleDateTimeUtc = "", string candleDateTimeKst = "",
        //    double openingPrice = double.NaN, double highPrice = double.NaN,
        //    double lowPrice = double.NaN, double tradePrice = double.NaN,
        //    decimal timestamp = default(decimal), double candleAccTradePrice = double.NaN,
        //    double candleAccTradeVolume = double.NaN, double prevClosingPrice = double.NaN,
        //    double changePrice = double.NaN, double changeRate = double.NaN,
        //    double convertedTradePrice = double.NaN)
        //{
        //    Market = market;
        //    CandleDateTimeUtc = candleDateTimeUtc;
        //    CandleDateTimeKst = candleDateTimeKst;
        //    OpeningPrice = openingPrice;
        //    HighPrice = highPrice;
        //    LowPrice = lowPrice;
        //    TradePrice = tradePrice;
        //    Timestamp = timestamp;
        //    CandleAccTradePrice = candleAccTradePrice;
        //    CandleAccTradeVolume = candleAccTradeVolume;
        //    PrevClosingPrice = prevClosingPrice;
        //    ChangePrice = changePrice;
        //    ChangeRate = changeRate;
        //    ConvertedTradePrice = convertedTradePrice;
        //}

        /// <summary>
        /// 마켓명
        /// </summary>
        /// <value>마켓명</value>
        [DataMember(Name = "market", EmitDefaultValue = false)]
        public string Market { get; set; } = "";

        /// <summary>
        /// 캔들 기준 시각 (UTC 기준)
        /// </summary>
        /// <value>캔들 기준 시각 (UTC 기준)</value>
        [DataMember(Name = "candle_date_time_utc", EmitDefaultValue = false)]
        public string CandleDateTimeUtc { get; set; } = "";

        /// <summary>
        /// 캔들 기준 시각 (KST 기준)
        /// </summary>
        /// <value>캔들 기준 시각 (KST 기준)</value>
        [DataMember(Name = "candle_date_time_kst", EmitDefaultValue = false)]
        public string CandleDateTimeKst { get; set; } = "";

        /// <summary>
        /// 시가
        /// </summary>
        /// <value>시가</value>
        [DataMember(Name = "opening_price", EmitDefaultValue = false)]
        public double OpeningPrice { get; set; }

        /// <summary>
        /// 고가
        /// </summary>
        /// <value>고가</value>
        [DataMember(Name = "high_price", EmitDefaultValue = false)]
        public double HighPrice { get; set; }

        /// <summary>
        /// 저가
        /// </summary>
        /// <value>저가</value>
        [DataMember(Name = "low_price", EmitDefaultValue = false)]
        public double LowPrice { get; set; }

        /// <summary>
        /// 종가
        /// </summary>
        /// <value>종가</value>
        [DataMember(Name = "trade_price", EmitDefaultValue = false)]
        public double TradePrice { get; set; }

        /// <summary>
        /// 해당 캔들에서 마지막 틱이 저장된 시각
        /// </summary>
        /// <value>해당 캔들에서 마지막 틱이 저장된 시각</value>
        [DataMember(Name = "timestamp", EmitDefaultValue = false)]
        public decimal Timestamp { get; set; }

        /// <summary>
        /// 누적 거래 금액
        /// </summary>
        /// <value>누적 거래 금액</value>
        [DataMember(Name = "candle_acc_trade_price", EmitDefaultValue = false)]
        public double CandleAccTradePrice { get; set; }

        /// <summary>
        /// 누적 거래량
        /// </summary>
        /// <value>누적 거래량</value>
        [DataMember(Name = "candle_acc_trade_volume", EmitDefaultValue = false)]
        public double CandleAccTradeVolume { get; set; }

        /// <summary>
        /// 전일 종가 (UTC 0시 기준)
        /// </summary>
        /// <value>전일 종가 (UTC 0시 기준)</value>
        [DataMember(Name = "prev_closing_price", EmitDefaultValue = false)]
        public double PrevClosingPrice { get; set; }

        /// <summary>
        /// 전일 종가 대비 변화 금액
        /// </summary>
        /// <value>전일 종가 대비 변화 금액</value>
        [DataMember(Name = "change_price", EmitDefaultValue = false)]
        public double ChangePrice { get; set; }

        /// <summary>
        /// 전일 종가 대비 변화량
        /// </summary>
        /// <value>전일 종가 대비 변화량</value>
        [DataMember(Name = "change_rate", EmitDefaultValue = false)]
        public double ChangeRate { get; set; }

        /// <summary>
        /// 종가 환산 화폐 단위로 환산된 가격 (요청에 convertingPriceUnit 파라미터 없을 시 해당 필드 포함되지 않음.) 
        /// </summary>
        /// <value>종가 환산 화폐 단위로 환산된 가격 (요청에 convertingPriceUnit 파라미터 없을 시 해당 필드 포함되지 않음.) </value>
        [DataMember(Name = "converted_trade_price", EmitDefaultValue = false)]
        public double? ConvertedTradePrice { get; set; }

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

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object? input) => (input as CandleDay)?.Equals(this) ?? false;

        /// <summary>
        /// Returns true if CandleDays instances are equal
        /// </summary>
        /// <param name="input">Instance of CandleDays to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(CandleDay? input)
        {
            if (input == null) return false;
            return
                (
                    Market == input.Market || (Market != null && Market.Equals(input.Market))
                ) &&
                (
                    CandleDateTimeUtc == input.CandleDateTimeUtc ||
                    (CandleDateTimeUtc != null && CandleDateTimeUtc.Equals(input.CandleDateTimeUtc))
                ) &&
                (
                    CandleDateTimeKst == input.CandleDateTimeKst ||
                    (CandleDateTimeKst != null && CandleDateTimeKst.Equals(input.CandleDateTimeKst))
                ) &&
                (
                    OpeningPrice == input.OpeningPrice || OpeningPrice.Equals(input.OpeningPrice)
                ) &&
                (
                    HighPrice == input.HighPrice || HighPrice.Equals(input.HighPrice)
                ) &&
                (
                    LowPrice == input.LowPrice || LowPrice.Equals(input.LowPrice)
                ) &&
                (
                    TradePrice == input.TradePrice || TradePrice.Equals(input.TradePrice)
                ) &&
                (
                    Timestamp == input.Timestamp || Timestamp.Equals(input.Timestamp)
                ) &&
                (
                    CandleAccTradePrice == input.CandleAccTradePrice || CandleAccTradePrice.Equals(input.CandleAccTradePrice)
                ) &&
                (
                    CandleAccTradeVolume == input.CandleAccTradeVolume || CandleAccTradeVolume.Equals(input.CandleAccTradeVolume)
                ) &&
                (
                    PrevClosingPrice == input.PrevClosingPrice || PrevClosingPrice.Equals(input.PrevClosingPrice)
                ) &&
                (
                    ChangePrice == input.ChangePrice || ChangePrice.Equals(input.ChangePrice)
                ) &&
                (
                    ChangeRate == input.ChangeRate || ChangeRate.Equals(input.ChangeRate)
                ) &&
                (
                    (ConvertedTradePrice != null) &&
                    (ConvertedTradePrice == input.ConvertedTradePrice || ConvertedTradePrice.Equals(input.ConvertedTradePrice))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (Market != null) hashCode = hashCode * 59 + Market.GetHashCode();
                if (CandleDateTimeUtc != null) hashCode = hashCode * 59 + CandleDateTimeUtc.GetHashCode();
                if (CandleDateTimeKst != null) hashCode = hashCode * 59 + CandleDateTimeKst.GetHashCode();
                hashCode = hashCode * 59 + OpeningPrice.GetHashCode();
                hashCode = hashCode * 59 + HighPrice.GetHashCode();
                hashCode = hashCode * 59 + LowPrice.GetHashCode();
                hashCode = hashCode * 59 + TradePrice.GetHashCode();
                hashCode = hashCode * 59 + Timestamp.GetHashCode();
                hashCode = hashCode * 59 + CandleAccTradePrice.GetHashCode();
                hashCode = hashCode * 59 + CandleAccTradeVolume.GetHashCode();
                hashCode = hashCode * 59 + PrevClosingPrice.GetHashCode();
                hashCode = hashCode * 59 + ChangePrice.GetHashCode();
                hashCode = hashCode * 59 + ChangeRate.GetHashCode();
                if(ConvertedTradePrice != null) hashCode = hashCode * 59 + ConvertedTradePrice.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }

    }//class
}
