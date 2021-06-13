using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

#pragma warning disable CS8618

namespace Universe.Coin.Upbit.Model
{
    using JS = JsonSerializer;

    public class Orderbook : IOrderbook
    {
        //public Orderbook()
        //{
        //    Market = "";
        //    //OrderbookUnits = null!;//null이 아닐경우 json deserialization 에러 (Newtonsoft)
        //    OrderbookUnits = new List<OrderbookUnit>();
        //}

        /// <summary>
        /// 마켓 코드
        /// </summary>
        /// <value>마켓 코드</value>
        [JsonPropertyName("market")]
        public string Market { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// 호가 생성 시각
        /// </summary>
        /// <value>호가 생성 시각</value>
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        /// <summary>
        /// 호가 매도 총 잔량
        /// </summary>
        /// <value>호가 매도 총 잔량</value>
        [JsonPropertyName("total_ask_size")]
        public decimal TotalAskSize { get; set; }

        /// <summary>
        /// 호가 매수 총량
        /// </summary>
        /// <value>호가 매수 총량</value>
        [JsonPropertyName("total_bid_size")]
        public decimal TotalBidSize { get; set; }

        /// <summary>
        /// 호가
        /// </summary>
        /// <value>호가</value>
        [JsonPropertyName("orderbook_units")]
        [JsonConverter(typeof(JcEnumerable<IOrderbookUnit, OrderbookUnit>))]
        public IEnumerable<IOrderbookUnit> OrderbookUnits { get; set; }


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

#pragma warning restore CS8618
