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
    public class Orderbook : IEquatable<Orderbook>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Orderbook" /> class.
        /// </summary>
        /// <param name="market">마켓 코드.</param>
        /// <param name="timestamp">호가 생성 시각.</param>
        /// <param name="totalAskSize">호가 매도 총 잔량.</param>
        /// <param name="totalBidSize">호가 매수 총량.</param>
        /// <param name="orderbookUnits">호가.</param>
        //public Orderbook(string market = default(string), decimal? timestamp = default(decimal?), double? totalAskSize = default(double?), double? totalBidSize = default(double?), List<OrderbookUnit> orderbookUnits = default(List<OrderbookUnit>))
        //{
        //    this.Market = market;
        //    this.Timestamp = timestamp;
        //    this.TotalAskSize = totalAskSize;
        //    this.TotalBidSize = totalBidSize;
        //    this.OrderbookUnits = orderbookUnits;
        //}

        /// <summary>
        /// 마켓 코드
        /// </summary>
        /// <value>마켓 코드</value>
        [DataMember(Name = "market", EmitDefaultValue = false)]
        public string Market { get; set; } = "";

        /// <summary>
        /// 호가 생성 시각
        /// </summary>
        /// <value>호가 생성 시각</value>
        [DataMember(Name = "timestamp", EmitDefaultValue = false)]
        public decimal? Timestamp { get; set; }

        /// <summary>
        /// 호가 매도 총 잔량
        /// </summary>
        /// <value>호가 매도 총 잔량</value>
        [DataMember(Name = "total_ask_size", EmitDefaultValue = false)]
        public double? TotalAskSize { get; set; }

        /// <summary>
        /// 호가 매수 총량
        /// </summary>
        /// <value>호가 매수 총량</value>
        [DataMember(Name = "total_bid_size", EmitDefaultValue = false)]
        public double? TotalBidSize { get; set; }

        /// <summary>
        /// 호가
        /// </summary>
        /// <value>호가</value>
        [DataMember(Name = "orderbook_units", EmitDefaultValue = false)]
        public List<OrderbookUnit> OrderbookUnits { get; set; }

        const int w = 20;
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Market:         {Market,w}");
            sb.AppendLine($"Timestamp:      {Timestamp,w}");
            sb.AppendLine($"TotalAskSize:   {TotalAskSize,w}");
            sb.AppendLine($"TotalBidSize:   {TotalBidSize,w}");
            sb.AppendLine($"OrderbookUnits: {OrderbookUnits,w}");
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
        public override bool Equals(object input) => (input as Orderbook)?.Equals(this) ?? false;

        /// <summary>
        /// Returns true if Orderbook instances are equal
        /// </summary>
        /// <param name="input">Instance of Orderbook to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Orderbook input)
        {
            if (input == null) return false;

            return
                (
                    this.Market == input.Market ||
                    (this.Market != null &&
                    this.Market.Equals(input.Market))
                ) &&
                (
                    this.Timestamp == input.Timestamp ||
                    (this.Timestamp != null &&
                    this.Timestamp.Equals(input.Timestamp))
                ) &&
                (
                    this.TotalAskSize == input.TotalAskSize ||
                    (this.TotalAskSize != null &&
                    this.TotalAskSize.Equals(input.TotalAskSize))
                ) &&
                (
                    this.TotalBidSize == input.TotalBidSize ||
                    (this.TotalBidSize != null &&
                    this.TotalBidSize.Equals(input.TotalBidSize))
                ) &&
                (
                    this.OrderbookUnits == input.OrderbookUnits ||
                    this.OrderbookUnits != null &&
                    this.OrderbookUnits.SequenceEqual(input.OrderbookUnits)
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
                if (this.Timestamp != null)
                    hashCode = hashCode * 59 + this.Timestamp.GetHashCode();
                if (this.TotalAskSize != null)
                    hashCode = hashCode * 59 + this.TotalAskSize.GetHashCode();
                if (this.TotalBidSize != null)
                    hashCode = hashCode * 59 + this.TotalBidSize.GetHashCode();
                if (this.OrderbookUnits != null)
                    hashCode = hashCode * 59 + this.OrderbookUnits.GetHashCode();
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
