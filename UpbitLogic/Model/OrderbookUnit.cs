using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.Upbit.Model
{
    public class OrderbookUnit : IOrderbookUnit
    {
        /// <summary>
        /// 매도호가
        /// </summary>
        /// <value>매도호가</value>
        [JsonPropertyName("ask_price")]
        public decimal AskPrice { get; set; }

        /// <summary>
        /// 매수호가
        /// </summary>
        /// <value>매수호가</value>
        [JsonPropertyName("bid_price")]
        public decimal BidPrice { get; set; }

        /// <summary>
        /// 매도 잔량
        /// </summary>
        /// <value>매도 잔량</value>
        [JsonPropertyName("ask_size")]
        public decimal AskSize { get; set; }

        /// <summary>
        /// 매수 잔량
        /// </summary>
        /// <value>매수 잔량</value>
        [JsonPropertyName("bid_size")]
        public decimal BidSize { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"AskPrice: {AskPrice,10}");
            sb.Append($"BidPrice: {BidPrice,10}");
            sb.Append($"AskSize:  {AskSize,10}");
            sb.Append($"BidSize:  {BidSize,10}");
            return sb.ToString();
        }

    }
}
