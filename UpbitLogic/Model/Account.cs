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
    /// <summary>
    /// 사용자의 계좌 정보
    /// </summary>
    [DataContract]
    public class Account
    {
        public Account() => Currency = UnitCurrency = string.Empty;

        /// <summary>
        /// 화폐를 의미하는 영문 대문자 코드
        /// </summary>
        /// <value>화폐를 의미하는 영문 대문자 코드</value>
        [DataMember(Name = "currency", EmitDefaultValue = false)]
        public string Currency { get; set; }

        /// <summary>
        /// 주문가능 금액/수량
        /// </summary>
        /// <value>주문가능 금액/수량</value>
        [DataMember(Name = "balance", EmitDefaultValue = false)]
        public decimal Balance { get; set; }

        /// <summary>
        /// 주문 중 묶여있는 금액/수량
        /// </summary>
        /// <value>주문 중 묶여있는 금액/수량</value>
        [DataMember(Name = "locked", EmitDefaultValue = false)]
        public decimal Locked { get; set; }

        /// <summary>
        /// 매수평균가
        /// </summary>
        /// <value>매수평균가</value>
        [DataMember(Name = "avg_buy_price", EmitDefaultValue = false)]
        public decimal AvgBuyPrice { get; set; }

        /// <summary>
        /// 매수평균가 수정 여부
        /// </summary>
        /// <value>매수평균가 수정 여부</value>
        [DataMember(Name = "avg_buy_price_modified", EmitDefaultValue = false)]
        public bool AvgBuyPriceModified { get; set; }

        /// <summary>
        /// 평단가 기준 화폐
        /// </summary>
        /// <value>평단가 기준 화폐</value>
        [DataMember(Name = "unit_currency", EmitDefaultValue = false)]
        public string UnitCurrency { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Currency:     {Currency}");
            sb.Append($"Balance:      {Balance}");
            sb.Append($"Locked:       {Locked}");
            sb.Append($"AvgBuyPrice:  {AvgBuyPrice}");
            sb.Append($"AVPModified:  {AvgBuyPriceModified}");
            sb.Append($"UnitCurrency: {UnitCurrency}");
            return sb.ToString();
        }

    }

}
