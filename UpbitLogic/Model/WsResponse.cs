using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.Upbit.Model
{

#pragma warning disable CS8618

    public class WsResponse : IWsResponse
    {
        [JsonPropertyName("code")]
        public string Market { get; set; }

        [JsonPropertyName("type"), JsonConverter(typeof(TradeEventJC))]
        public TradeEvent Event { get; set; }

        [JsonPropertyName("stream_type")]
        public string StreamType { get; set; }

    }

#pragma warning restore CS8618

    public class TradeEventJC : JcEnum<TradeEventJC, TradeEvent>
    {
        static readonly string[] _names = { "ticker", "orderbook", "trade" };//순서: Ticker, Order, Trade
        static TradeEventJC() => init(_names);
    }

    public class WsResponseJC : JcInterface<IWsResponse>
    {
        public WsResponseJC() : base(typeof(WsResponseJC))
        {
        }
    }

}
