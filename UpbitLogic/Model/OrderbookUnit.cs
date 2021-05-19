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
    public class OrderbookUnit : IEquatable<OrderbookUnit>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderbookUnit" /> class.
        /// </summary>
        /// <param name="askPrice">매도호가.</param>
        /// <param name="bidPrice">매수호가.</param>
        /// <param name="askSize">매도 잔량.</param>
        /// <param name="bidSize">매수 잔량.</param>
        //public OrderbookUnit(double? askPrice = default(double?), double? bidPrice = default(double?), double? askSize = default(double?), double? bidSize = default(double?))
        //{
        //    this.AskPrice = askPrice;
        //    this.BidPrice = bidPrice;
        //    this.AskSize = askSize;
        //    this.BidSize = bidSize;
        //}

        /// <summary>
        /// 매도호가
        /// </summary>
        /// <value>매도호가</value>
        [DataMember(Name = "ask_price", EmitDefaultValue = false)]
        public double AskPrice { get; set; }

        /// <summary>
        /// 매수호가
        /// </summary>
        /// <value>매수호가</value>
        [DataMember(Name = "bid_price", EmitDefaultValue = false)]
        public double BidPrice { get; set; }

        /// <summary>
        /// 매도 잔량
        /// </summary>
        /// <value>매도 잔량</value>
        [DataMember(Name = "ask_size", EmitDefaultValue = false)]
        public double AskSize { get; set; }

        /// <summary>
        /// 매수 잔량
        /// </summary>
        /// <value>매수 잔량</value>
        [DataMember(Name = "bid_size", EmitDefaultValue = false)]
        public double BidSize { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"AskPrice: {AskPrice,10}");
            sb.Append($"BidPrice: {BidPrice,10}");
            sb.Append($"AskSize: {AskSize,10}");
            sb.Append($"BidSize: {BidSize,10}");
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
        public override bool Equals(object? input) => (input as OrderbookUnit)?.Equals(this) ?? false;

        /// <summary>
        /// Returns true if OrderbookUnit instances are equal
        /// </summary>
        /// <param name="input">Instance of OrderbookUnit to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(OrderbookUnit? input)
        {
            if (input == null) return false;

            return
                (
                    this.AskPrice == input.AskPrice ||
                    (this.AskPrice != null &&
                    this.AskPrice.Equals(input.AskPrice))
                ) &&
                (
                    this.BidPrice == input.BidPrice ||
                    (this.BidPrice != null &&
                    this.BidPrice.Equals(input.BidPrice))
                ) &&
                (
                    this.AskSize == input.AskSize ||
                    (this.AskSize != null &&
                    this.AskSize.Equals(input.AskSize))
                ) &&
                (
                    this.BidSize == input.BidSize ||
                    (this.BidSize != null &&
                    this.BidSize.Equals(input.BidSize))
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
                if (this.AskPrice != null)
                    hashCode = hashCode * 59 + this.AskPrice.GetHashCode();
                if (this.BidPrice != null)
                    hashCode = hashCode * 59 + this.BidPrice.GetHashCode();
                if (this.AskSize != null)
                    hashCode = hashCode * 59 + this.AskSize.GetHashCode();
                if (this.BidSize != null)
                    hashCode = hashCode * 59 + this.BidSize.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
