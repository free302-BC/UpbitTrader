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
    public class Orderbook : IApiModel
    {
        public Orderbook()
        {
            Market = "";
            OrderbookUnits = new List<OrderbookUnit>();
        }
                
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
        public long Timestamp { get; set; }

        /// <summary>
        /// 호가 매도 총 잔량
        /// </summary>
        /// <value>호가 매도 총 잔량</value>
        [DataMember(Name = "total_ask_size", EmitDefaultValue = false)]
        public decimal TotalAskSize { get; set; }

        /// <summary>
        /// 호가 매수 총량
        /// </summary>
        /// <value>호가 매수 총량</value>
        [DataMember(Name = "total_bid_size", EmitDefaultValue = false)]
        public decimal TotalBidSize { get; set; }

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
    }
}
