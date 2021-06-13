using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Model;
using Xunit;

namespace UnitTester
{
    public class WsResponseTeter
    {
        public class Res : IWsResponse
        {
            [JsonPropertyName("code")]
            public string Market { get; set; }

            [JsonPropertyName("type1"), JsonConverter(typeof(ResJC1))]
            public TradeEvent Event { get; set; }
            
            [JsonPropertyName("type2"), JsonConverter(typeof(ResJC2))]
            public TradeEvent Event2 { get; set; }

            [JsonPropertyName("stream_type")]
            public string StreamType { get; set; }
        }

        public class ResJC1 : JcEnum<ResJC1, TradeEvent>
        {
            static ResJC1() => init(new[] { "ticker", "orderbook", "trade" });
        }
        public class ResJC2 : JcEnum<ResJC2, TradeEvent>
        {
            static ResJC2() => init(new[] { "ticker", "order2", "trade" });
        }

        string _jsonR1 = "{\"code\": \"market1\", \"stream_type\": \"realtime\", \"type1\": \"orderbook\"}";
        string _jsonR2 = "{\"code\": \"market1\", \"stream_type\": \"realtime\", \"type2\": \"order2\"}";

        [Fact]
        void json_TradeEvent()
        {
            var r1 = JsonSerializer.Deserialize<Res>(_jsonR1);
            Assert.Equal(TradeEvent.Order, r1.Event);
            var r2 = JsonSerializer.Deserialize<Res>(_jsonR2);
            Assert.Equal(TradeEvent.Order, r2.Event2);

            var j1 = JsonSerializer.Serialize(r1);
            Assert.Contains("orderbook", j1);
            var j2 = JsonSerializer.Serialize(r2);
            Assert.Contains("order2", j2);
        }

    }
}
