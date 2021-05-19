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
    /// 사용자의 계좌 정보
    /// </summary>
    [DataContract]
    public partial class Account : IEquatable<Account>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Account" /> class.
        /// </summary>
        /// <param name="currency">화폐를 의미하는 영문 대문자 코드.</param>
        /// <param name="balance">주문가능 금액/수량.</param>
        /// <param name="locked">주문 중 묶여있는 금액/수량.</param>
        /// <param name="avgBuyPrice">매수평균가.</param>
        /// <param name="avgBuyPriceModified">매수평균가 수정 여부.</param>
        /// <param name="unitCurrency">평단가 기준 화폐.</param>
        //public Account(string currency = default(string), string balance = default(string), string locked = default(string), string avgBuyPrice = default(string), bool? avgBuyPriceModified = default(bool?), string unitCurrency = default(string))
        //{
        //    this.Currency = currency;
        //    this.Balance = balance;
        //    this.Locked = locked;
        //    this.AvgBuyPrice = avgBuyPrice;
        //    this.AvgBuyPriceModified = avgBuyPriceModified;
        //    this.UnitCurrency = unitCurrency;
        //}

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
        public string Balance { get; set; }

        /// <summary>
        /// 주문 중 묶여있는 금액/수량
        /// </summary>
        /// <value>주문 중 묶여있는 금액/수량</value>
        [DataMember(Name = "locked", EmitDefaultValue = false)]
        public string Locked { get; set; }

        /// <summary>
        /// 매수평균가
        /// </summary>
        /// <value>매수평균가</value>
        [DataMember(Name = "avg_buy_price", EmitDefaultValue = false)]
        public string AvgBuyPrice { get; set; }

        /// <summary>
        /// 매수평균가 수정 여부
        /// </summary>
        /// <value>매수평균가 수정 여부</value>
        [DataMember(Name = "avg_buy_price_modified", EmitDefaultValue = false)]
        public bool? AvgBuyPriceModified { get; set; }

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
        public override bool Equals(object? input) => (input as Account)?.Equals(this) ?? false;

        /// <summary>
        /// Returns true if Account instances are equal
        /// </summary>
        /// <param name="input">Instance of Account to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Account? input)
        {
            if (input == null) return false;

            return
                (
                    this.Currency == input.Currency ||
                    (this.Currency != null &&
                    this.Currency.Equals(input.Currency))
                ) &&
                (
                    this.Balance == input.Balance ||
                    (this.Balance != null &&
                    this.Balance.Equals(input.Balance))
                ) &&
                (
                    this.Locked == input.Locked ||
                    (this.Locked != null &&
                    this.Locked.Equals(input.Locked))
                ) &&
                (
                    this.AvgBuyPrice == input.AvgBuyPrice ||
                    (this.AvgBuyPrice != null &&
                    this.AvgBuyPrice.Equals(input.AvgBuyPrice))
                ) &&
                (
                    this.AvgBuyPriceModified == input.AvgBuyPriceModified ||
                    (this.AvgBuyPriceModified != null &&
                    this.AvgBuyPriceModified.Equals(input.AvgBuyPriceModified))
                ) &&
                (
                    this.UnitCurrency == input.UnitCurrency ||
                    (this.UnitCurrency != null &&
                    this.UnitCurrency.Equals(input.UnitCurrency))
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
                if (this.Currency != null)
                    hashCode = hashCode * 59 + this.Currency.GetHashCode();
                if (this.Balance != null)
                    hashCode = hashCode * 59 + this.Balance.GetHashCode();
                if (this.Locked != null)
                    hashCode = hashCode * 59 + this.Locked.GetHashCode();
                if (this.AvgBuyPrice != null)
                    hashCode = hashCode * 59 + this.AvgBuyPrice.GetHashCode();
                if (this.AvgBuyPriceModified != null)
                    hashCode = hashCode * 59 + this.AvgBuyPriceModified.GetHashCode();
                if (this.UnitCurrency != null)
                    hashCode = hashCode * 59 + this.UnitCurrency.GetHashCode();
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
