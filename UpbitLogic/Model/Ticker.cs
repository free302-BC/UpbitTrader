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
    public class Ticker : IApiModel
    {
        public Ticker()
            => Market = TradeDate = TradeTime = TradeDateKst = TradeTimeKst 
                = Change = Highest52WeekDate = Lowest52WeekDate = string.Empty;

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
        /// 전일 종가
        /// </summary>
        /// <value>전일 종가</value>
        [DataMember(Name = "prev_closing_price", EmitDefaultValue = false)]
        public decimal PrevClosingPrice { get; set; }

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
        public decimal ChangePrice { get; set; }

        /// <summary>
        /// 변화율의 절대값
        /// </summary>
        /// <value>변화율의 절대값</value>
        [DataMember(Name = "change_rate", EmitDefaultValue = false)]
        public decimal ChangeRate { get; set; }

        /// <summary>
        /// 부호가 있는 변화액
        /// </summary>
        /// <value>부호가 있는 변화액</value>
        [DataMember(Name = "signed_change_price", EmitDefaultValue = false)]
        public decimal SignedChangePrice { get; set; }

        /// <summary>
        /// 부호가 있는 변화율
        /// </summary>
        /// <value>부호가 있는 변화율</value>
        [DataMember(Name = "signed_change_rate", EmitDefaultValue = false)]
        public decimal SignedChangeRate { get; set; }

        /// <summary>
        /// 가장 최근 거래량
        /// </summary>
        /// <value>가장 최근 거래량</value>
        [DataMember(Name = "trade_volume", EmitDefaultValue = false)]
        public decimal TradeVolume { get; set; }

        /// <summary>
        /// 누적 거래대금 (UTC 0시 기준)
        /// </summary>
        /// <value>누적 거래대금 (UTC 0시 기준)</value>
        [DataMember(Name = "acc_trade_price", EmitDefaultValue = false)]
        public decimal AccTradePrice { get; set; }

        /// <summary>
        /// 24시간 누적 거래대금
        /// </summary>
        /// <value>24시간 누적 거래대금</value>
        [DataMember(Name = "acc_trade_price_24h", EmitDefaultValue = false)]
        public decimal AccTradePrice24h { get; set; }

        /// <summary>
        /// 누적 거래량 (UTC 0시 기준)
        /// </summary>
        /// <value>누적 거래량 (UTC 0시 기준)</value>
        [DataMember(Name = "acc_trade_volume", EmitDefaultValue = false)]
        public decimal AccTradeVolume { get; set; }

        /// <summary>
        /// 24시간 누적 거래량
        /// </summary>
        /// <value>24시간 누적 거래량</value>
        [DataMember(Name = "acc_trade_volume_24h", EmitDefaultValue = false)]
        public decimal AccTradeVolume24h { get; set; }

        /// <summary>
        /// 52주 신고가
        /// </summary>
        /// <value>52주 신고가</value>
        [DataMember(Name = "highest_52_week_price", EmitDefaultValue = false)]
        public decimal Highest52WeekPrice { get; set; }

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
        public decimal Lowest52WeekPrice { get; set; }

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
        public long Timestamp { get; set; }

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

    }

}
