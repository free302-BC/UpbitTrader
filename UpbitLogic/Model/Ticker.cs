﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit.Model
{
    /// <summary>
    /// Ticker
    /// </summary>
    [DataContract]
    public partial class Ticker : IEquatable<Ticker>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Ticker" /> class.
        /// </summary>
        /// <param name="market">종목 구분 코드.</param>
        /// <param name="tradeDate">최근 거래 일자(UTC).</param>
        /// <param name="tradeTime">최근 거래 시각(UTC).</param>
        /// <param name="tradeDateKst">최근 거래 일자(KST).</param>
        /// <param name="tradeTimeKst">최근 거래 시각(KST).</param>
        /// <param name="openingPrice">시가.</param>
        /// <param name="highPrice">고가.</param>
        /// <param name="lowPrice">저가.</param>
        /// <param name="tradePrice">종가.</param>
        /// <param name="prevClosingPrice">전일 종가.</param>
        /// <param name="change">EVEN : 보합 RISE : 상승 FALL : 하락 .</param>
        /// <param name="changePrice">변화액의 절대값.</param>
        /// <param name="changeRate">변화율의 절대값.</param>
        /// <param name="signedChangePrice">부호가 있는 변화액.</param>
        /// <param name="signedChangeRate">부호가 있는 변화율.</param>
        /// <param name="tradeVolume">가장 최근 거래량.</param>
        /// <param name="accTradePrice">누적 거래대금 (UTC 0시 기준).</param>
        /// <param name="accTradePrice24h">24시간 누적 거래대금.</param>
        /// <param name="accTradeVolume">누적 거래량 (UTC 0시 기준).</param>
        /// <param name="accTradeVolume24h">24시간 누적 거래량.</param>
        /// <param name="highest52WeekPrice">52주 신고가.</param>
        /// <param name="highest52WeekDate">52주 신고가 달성일.</param>
        /// <param name="lowest52WeekPrice">52주 신저가.</param>
        /// <param name="lowest52WeekDate">52주 신저가 달성일.</param>
        /// <param name="timestamp">타임스탬프.</param>
        //public Ticker(string market = default(string), string tradeDate = default(string), string tradeTime = default(string), string tradeDateKst = default(string), string tradeTimeKst = default(string), double? openingPrice = default(double?), double? highPrice = default(double?), double? lowPrice = default(double?), double? tradePrice = default(double?), double? prevClosingPrice = default(double?), string change = default(string), double? changePrice = default(double?), double? changeRate = default(double?), double? signedChangePrice = default(double?), double? signedChangeRate = default(double?), double? tradeVolume = default(double?), double? accTradePrice = default(double?), double? accTradePrice24h = default(double?), double? accTradeVolume = default(double?), double? accTradeVolume24h = default(double?), double? highest52WeekPrice = default(double?), string highest52WeekDate = default(string), double? lowest52WeekPrice = default(double?), string lowest52WeekDate = default(string), decimal? timestamp = default(decimal?))
        //{
        //    this.Market = market;
        //    this.TradeDate = tradeDate;
        //    this.TradeTime = tradeTime;
        //    this.TradeDateKst = tradeDateKst;
        //    this.TradeTimeKst = tradeTimeKst;
        //    this.OpeningPrice = openingPrice;
        //    this.HighPrice = highPrice;
        //    this.LowPrice = lowPrice;
        //    this.TradePrice = tradePrice;
        //    this.PrevClosingPrice = prevClosingPrice;
        //    this.Change = change;
        //    this.ChangePrice = changePrice;
        //    this.ChangeRate = changeRate;
        //    this.SignedChangePrice = signedChangePrice;
        //    this.SignedChangeRate = signedChangeRate;
        //    this.TradeVolume = tradeVolume;
        //    this.AccTradePrice = accTradePrice;
        //    this.AccTradePrice24h = accTradePrice24h;
        //    this.AccTradeVolume = accTradeVolume;
        //    this.AccTradeVolume24h = accTradeVolume24h;
        //    this.Highest52WeekPrice = highest52WeekPrice;
        //    this.Highest52WeekDate = highest52WeekDate;
        //    this.Lowest52WeekPrice = lowest52WeekPrice;
        //    this.Lowest52WeekDate = lowest52WeekDate;
        //    this.Timestamp = timestamp;
        //}

        /// <summary>
        /// 종목 구분 코드
        /// </summary>
        /// <value>종목 구분 코드</value>
        [DataMember(Name = "market", EmitDefaultValue = false)]
        public string Market { get; set; }

        /// <summary>
        /// 최근 거래 일자(UTC)
        /// </summary>
        /// <value>최근 거래 일자(UTC)</value>
        [DataMember(Name = "trade_date", EmitDefaultValue = false)]
        public string TradeDate { get; set; }

        /// <summary>
        /// 최근 거래 시각(UTC)
        /// </summary>
        /// <value>최근 거래 시각(UTC)</value>
        [DataMember(Name = "trade_time", EmitDefaultValue = false)]
        public string TradeTime { get; set; }

        /// <summary>
        /// 최근 거래 일자(KST)
        /// </summary>
        /// <value>최근 거래 일자(KST)</value>
        [DataMember(Name = "trade_date_kst", EmitDefaultValue = false)]
        public string TradeDateKst { get; set; }

        /// <summary>
        /// 최근 거래 시각(KST)
        /// </summary>
        /// <value>최근 거래 시각(KST)</value>
        [DataMember(Name = "trade_time_kst", EmitDefaultValue = false)]
        public string TradeTimeKst { get; set; }

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
        /// 전일 종가
        /// </summary>
        /// <value>전일 종가</value>
        [DataMember(Name = "prev_closing_price", EmitDefaultValue = false)]
        public double PrevClosingPrice { get; set; }

        /// <summary>
        /// EVEN : 보합 RISE : 상승 FALL : 하락 
        /// </summary>
        /// <value>EVEN : 보합 RISE : 상승 FALL : 하락 </value>
        [DataMember(Name = "change", EmitDefaultValue = false)]
        public string Change { get; set; }

        /// <summary>
        /// 변화액의 절대값
        /// </summary>
        /// <value>변화액의 절대값</value>
        [DataMember(Name = "change_price", EmitDefaultValue = false)]
        public double ChangePrice { get; set; }

        /// <summary>
        /// 변화율의 절대값
        /// </summary>
        /// <value>변화율의 절대값</value>
        [DataMember(Name = "change_rate", EmitDefaultValue = false)]
        public double ChangeRate { get; set; }

        /// <summary>
        /// 부호가 있는 변화액
        /// </summary>
        /// <value>부호가 있는 변화액</value>
        [DataMember(Name = "signed_change_price", EmitDefaultValue = false)]
        public double SignedChangePrice { get; set; }

        /// <summary>
        /// 부호가 있는 변화율
        /// </summary>
        /// <value>부호가 있는 변화율</value>
        [DataMember(Name = "signed_change_rate", EmitDefaultValue = false)]
        public double SignedChangeRate { get; set; }

        /// <summary>
        /// 가장 최근 거래량
        /// </summary>
        /// <value>가장 최근 거래량</value>
        [DataMember(Name = "trade_volume", EmitDefaultValue = false)]
        public double TradeVolume { get; set; }

        /// <summary>
        /// 누적 거래대금 (UTC 0시 기준)
        /// </summary>
        /// <value>누적 거래대금 (UTC 0시 기준)</value>
        [DataMember(Name = "acc_trade_price", EmitDefaultValue = false)]
        public double AccTradePrice { get; set; }

        /// <summary>
        /// 24시간 누적 거래대금
        /// </summary>
        /// <value>24시간 누적 거래대금</value>
        [DataMember(Name = "acc_trade_price_24h", EmitDefaultValue = false)]
        public double AccTradePrice24h { get; set; }

        /// <summary>
        /// 누적 거래량 (UTC 0시 기준)
        /// </summary>
        /// <value>누적 거래량 (UTC 0시 기준)</value>
        [DataMember(Name = "acc_trade_volume", EmitDefaultValue = false)]
        public double AccTradeVolume { get; set; }

        /// <summary>
        /// 24시간 누적 거래량
        /// </summary>
        /// <value>24시간 누적 거래량</value>
        [DataMember(Name = "acc_trade_volume_24h", EmitDefaultValue = false)]
        public double AccTradeVolume24h { get; set; }

        /// <summary>
        /// 52주 신고가
        /// </summary>
        /// <value>52주 신고가</value>
        [DataMember(Name = "highest_52_week_price", EmitDefaultValue = false)]
        public double Highest52WeekPrice { get; set; }

        /// <summary>
        /// 52주 신고가 달성일
        /// </summary>
        /// <value>52주 신고가 달성일</value>
        [DataMember(Name = "highest_52_week_date", EmitDefaultValue = false)]
        public string Highest52WeekDate { get; set; }

        /// <summary>
        /// 52주 신저가
        /// </summary>
        /// <value>52주 신저가</value>
        [DataMember(Name = "lowest_52_week_price", EmitDefaultValue = false)]
        public double Lowest52WeekPrice { get; set; }

        /// <summary>
        /// 52주 신저가 달성일
        /// </summary>
        /// <value>52주 신저가 달성일</value>
        [DataMember(Name = "lowest_52_week_date", EmitDefaultValue = false)]
        public string Lowest52WeekDate { get; set; }

        /// <summary>
        /// 타임스탬프
        /// </summary>
        /// <value>타임스탬프</value>
        [DataMember(Name = "timestamp", EmitDefaultValue = false)]
        public decimal Timestamp { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Market:             {Market}");
            sb.AppendLine($"TradeDate:          {TradeDate}");
            sb.AppendLine($"TradeTime:          {TradeTime}");
            sb.AppendLine($"TradeDateKst:       {TradeDateKst}");
            sb.AppendLine($"TradeTimeKst:       {TradeTimeKst}");
            sb.AppendLine($"OpeningPrice:       {OpeningPrice}");
            sb.AppendLine($"HighPrice:          {HighPrice}");
            sb.AppendLine($"LowPrice:           {LowPrice}");
            sb.AppendLine($"TradePrice:         {TradePrice}");
            sb.AppendLine($"PrevClosingPrice:   {PrevClosingPrice}");
            sb.AppendLine($"Change:             {Change}");
            sb.AppendLine($"ChangePrice:        {ChangePrice}");
            sb.AppendLine($"ChangeRate:         {ChangeRate}");
            sb.AppendLine($"SignedChangePrice:  {SignedChangePrice}");
            sb.AppendLine($"SignedChangeRate:   {SignedChangeRate}");
            sb.AppendLine($"TradeVolume:        {TradeVolume}");
            sb.AppendLine($"AccTradePrice:      {AccTradePrice}");
            sb.AppendLine($"AccTradePrice24h:   {AccTradePrice24h}");
            sb.AppendLine($"AccTradeVolume:     {AccTradeVolume}");
            sb.AppendLine($"AccTradeVolume24h:  {AccTradeVolume24h}");
            sb.AppendLine($"Highest52WeekPrice: {Highest52WeekPrice}");
            sb.AppendLine($"Highest52WeekDate:  {Highest52WeekDate}");
            sb.AppendLine($"Lowest52WeekPrice:  {Lowest52WeekPrice}");
            sb.AppendLine($"Lowest52WeekDate:   {Lowest52WeekDate}");
            sb.AppendLine($"Timestamp:          {Timestamp}");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object? input)
        {
            return (input as Ticker)?.Equals(this) ?? false;
        }

        /// <summary>
        /// Returns true if Ticker instances are equal
        /// </summary>
        /// <param name="input">Instance of Ticker to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Ticker? input)
        {
            if (input == null) return false;

            return
                (
                    this.Market == input.Market ||
                    (this.Market != null &&
                    this.Market.Equals(input.Market))
                ) &&
                (
                    this.TradeDate == input.TradeDate ||
                    (this.TradeDate != null &&
                    this.TradeDate.Equals(input.TradeDate))
                ) &&
                (
                    this.TradeTime == input.TradeTime ||
                    (this.TradeTime != null &&
                    this.TradeTime.Equals(input.TradeTime))
                ) &&
                (
                    this.TradeDateKst == input.TradeDateKst ||
                    (this.TradeDateKst != null &&
                    this.TradeDateKst.Equals(input.TradeDateKst))
                ) &&
                (
                    this.TradeTimeKst == input.TradeTimeKst ||
                    (this.TradeTimeKst != null &&
                    this.TradeTimeKst.Equals(input.TradeTimeKst))
                ) &&
                (
                    this.OpeningPrice == input.OpeningPrice ||
                    (this.OpeningPrice != null &&
                    this.OpeningPrice.Equals(input.OpeningPrice))
                ) &&
                (
                    this.HighPrice == input.HighPrice ||
                    (this.HighPrice != null &&
                    this.HighPrice.Equals(input.HighPrice))
                ) &&
                (
                    this.LowPrice == input.LowPrice ||
                    (this.LowPrice != null &&
                    this.LowPrice.Equals(input.LowPrice))
                ) &&
                (
                    this.TradePrice == input.TradePrice ||
                    (this.TradePrice != null &&
                    this.TradePrice.Equals(input.TradePrice))
                ) &&
                (
                    this.PrevClosingPrice == input.PrevClosingPrice ||
                    (this.PrevClosingPrice != null &&
                    this.PrevClosingPrice.Equals(input.PrevClosingPrice))
                ) &&
                (
                    this.Change == input.Change ||
                    (this.Change != null &&
                    this.Change.Equals(input.Change))
                ) &&
                (
                    this.ChangePrice == input.ChangePrice ||
                    (this.ChangePrice != null &&
                    this.ChangePrice.Equals(input.ChangePrice))
                ) &&
                (
                    this.ChangeRate == input.ChangeRate ||
                    (this.ChangeRate != null &&
                    this.ChangeRate.Equals(input.ChangeRate))
                ) &&
                (
                    this.SignedChangePrice == input.SignedChangePrice ||
                    (this.SignedChangePrice != null &&
                    this.SignedChangePrice.Equals(input.SignedChangePrice))
                ) &&
                (
                    this.SignedChangeRate == input.SignedChangeRate ||
                    (this.SignedChangeRate != null &&
                    this.SignedChangeRate.Equals(input.SignedChangeRate))
                ) &&
                (
                    this.TradeVolume == input.TradeVolume ||
                    (this.TradeVolume != null &&
                    this.TradeVolume.Equals(input.TradeVolume))
                ) &&
                (
                    this.AccTradePrice == input.AccTradePrice ||
                    (this.AccTradePrice != null &&
                    this.AccTradePrice.Equals(input.AccTradePrice))
                ) &&
                (
                    this.AccTradePrice24h == input.AccTradePrice24h ||
                    (this.AccTradePrice24h != null &&
                    this.AccTradePrice24h.Equals(input.AccTradePrice24h))
                ) &&
                (
                    this.AccTradeVolume == input.AccTradeVolume ||
                    (this.AccTradeVolume != null &&
                    this.AccTradeVolume.Equals(input.AccTradeVolume))
                ) &&
                (
                    this.AccTradeVolume24h == input.AccTradeVolume24h ||
                    (this.AccTradeVolume24h != null &&
                    this.AccTradeVolume24h.Equals(input.AccTradeVolume24h))
                ) &&
                (
                    this.Highest52WeekPrice == input.Highest52WeekPrice ||
                    (this.Highest52WeekPrice != null &&
                    this.Highest52WeekPrice.Equals(input.Highest52WeekPrice))
                ) &&
                (
                    this.Highest52WeekDate == input.Highest52WeekDate ||
                    (this.Highest52WeekDate != null &&
                    this.Highest52WeekDate.Equals(input.Highest52WeekDate))
                ) &&
                (
                    this.Lowest52WeekPrice == input.Lowest52WeekPrice ||
                    (this.Lowest52WeekPrice != null &&
                    this.Lowest52WeekPrice.Equals(input.Lowest52WeekPrice))
                ) &&
                (
                    this.Lowest52WeekDate == input.Lowest52WeekDate ||
                    (this.Lowest52WeekDate != null &&
                    this.Lowest52WeekDate.Equals(input.Lowest52WeekDate))
                ) &&
                (
                    this.Timestamp == input.Timestamp ||
                    (this.Timestamp != null &&
                    this.Timestamp.Equals(input.Timestamp))
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
                if (this.Market != null)
                    hashCode = hashCode * 59 + this.Market.GetHashCode();
                if (this.TradeDate != null)
                    hashCode = hashCode * 59 + this.TradeDate.GetHashCode();
                if (this.TradeTime != null)
                    hashCode = hashCode * 59 + this.TradeTime.GetHashCode();
                if (this.TradeDateKst != null)
                    hashCode = hashCode * 59 + this.TradeDateKst.GetHashCode();
                if (this.TradeTimeKst != null)
                    hashCode = hashCode * 59 + this.TradeTimeKst.GetHashCode();
                if (this.OpeningPrice != null)
                    hashCode = hashCode * 59 + this.OpeningPrice.GetHashCode();
                if (this.HighPrice != null)
                    hashCode = hashCode * 59 + this.HighPrice.GetHashCode();
                if (this.LowPrice != null)
                    hashCode = hashCode * 59 + this.LowPrice.GetHashCode();
                if (this.TradePrice != null)
                    hashCode = hashCode * 59 + this.TradePrice.GetHashCode();
                if (this.PrevClosingPrice != null)
                    hashCode = hashCode * 59 + this.PrevClosingPrice.GetHashCode();
                if (this.Change != null)
                    hashCode = hashCode * 59 + this.Change.GetHashCode();
                if (this.ChangePrice != null)
                    hashCode = hashCode * 59 + this.ChangePrice.GetHashCode();
                if (this.ChangeRate != null)
                    hashCode = hashCode * 59 + this.ChangeRate.GetHashCode();
                if (this.SignedChangePrice != null)
                    hashCode = hashCode * 59 + this.SignedChangePrice.GetHashCode();
                if (this.SignedChangeRate != null)
                    hashCode = hashCode * 59 + this.SignedChangeRate.GetHashCode();
                if (this.TradeVolume != null)
                    hashCode = hashCode * 59 + this.TradeVolume.GetHashCode();
                if (this.AccTradePrice != null)
                    hashCode = hashCode * 59 + this.AccTradePrice.GetHashCode();
                if (this.AccTradePrice24h != null)
                    hashCode = hashCode * 59 + this.AccTradePrice24h.GetHashCode();
                if (this.AccTradeVolume != null)
                    hashCode = hashCode * 59 + this.AccTradeVolume.GetHashCode();
                if (this.AccTradeVolume24h != null)
                    hashCode = hashCode * 59 + this.AccTradeVolume24h.GetHashCode();
                if (this.Highest52WeekPrice != null)
                    hashCode = hashCode * 59 + this.Highest52WeekPrice.GetHashCode();
                if (this.Highest52WeekDate != null)
                    hashCode = hashCode * 59 + this.Highest52WeekDate.GetHashCode();
                if (this.Lowest52WeekPrice != null)
                    hashCode = hashCode * 59 + this.Lowest52WeekPrice.GetHashCode();
                if (this.Lowest52WeekDate != null)
                    hashCode = hashCode * 59 + this.Lowest52WeekDate.GetHashCode();
                if (this.Timestamp != null)
                    hashCode = hashCode * 59 + this.Timestamp.GetHashCode();
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
    }

}
