using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;
using Universe.Coin.Upbit;
using Universe.Coin.Upbit.Model;
using Xunit;

namespace UnitTester
{
    using JN = Newtonsoft.Json.JsonConvert;
    using JS = System.Text.Json.JsonSerializer;

    public class JsonTester
    {
        const string tickJson
            = "{\"type\":\"trade\",\"code\":\"KRW-BTC\",\"timestamp\":1623002455634,"
            + "\"trade_date\":\"2021-06-06\",\"trade_time\":\"18:00:55\",\"trade_timestamp\":1623002455000,"
            + "\"trade_price\":41998000.0,\"trade_volume\":0.00022715,\"ask_bid\":\"BID\","
            + "\"prev_closing_price\":3.141592E+5,\"change\":\"RISE\",\"change_price\":55000.00000000,"
            + "\"sequential_id\":1623002455000000,\"stream_type\":\"REALTIME\"}";

        [Fact] void Orderbook()
        {
            var json = File.ReadAllText("orderbook.json");
            //var opt = Helper.GetJsonOptions();
            //opt.Converters.Add(new JcOrderbookUnits());

            var ob = JS.Deserialize<Orderbook>(json);//, opt);
            Assert.NotEmpty(ob.OrderbookUnits);
        }


        [Fact] void JsonPropertyName()
        {
            var opt = new JsonSerializerOptions();
            opt.IncludeFields = true;
            opt.PropertyNameCaseInsensitive = false;

            var response = JS.Deserialize<WsResponse>(tickJson, opt);
            Assert.Equal("trade", response.requestType);
        }

        [Fact]
        void speed_Newton_vs_System()
        {
            var sw = Stopwatch.StartNew();
            var numLoop = 100000;
            var opt = new JsonSerializerOptions();
            opt.IncludeFields = true;

            void run(string name, Action< string> serialzer)
            {
                sw.Restart();
                for (int i = 0; i < numLoop; i++) serialzer(tickJson);
                Debug.WriteLine("{0}: {1}ms", name, sw.ElapsedMilliseconds);
            }

            run("Newton", json => JN.DeserializeObject<WsResponse>(json));
            run("System", json => JS.Deserialize<WsResponse2>(json));

            run("Newton", json => JN.DeserializeObject<WsResponse>(json));
            run("System", json => JS.Deserialize<WsResponse2>(json));

            //var js = JS.Deserialize<WsResponse2>(json, opt);
            //var type = response.requestType;
        }

        [DataContract]
        public class WsResponse2
        {
            [DataMember(Name = "type")] public string type;// { get; set; }
            [DataMember(Name = "code")] public string code;// { get; set; }
            [DataMember(Name = "stream_type")] public string stream_type;// { get; set; }
        }
    }
}
