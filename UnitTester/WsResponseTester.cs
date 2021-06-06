using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;
using Universe.Coin.Upbit.Model;
using Xunit;

namespace UnitTester
{
    using JN = Newtonsoft.Json.JsonConvert;
    using JS = System.Text.Json.JsonSerializer;
    using JU = Utf8Json.JsonSerializer;

    public class WsResponseTester
    {
        const string json
            = "{\"type\":\"trade\",\"code\":\"KRW-BTC\",\"timestamp\":1623002455634,"
            + "\"trade_date\":\"2021-06-06\",\"trade_time\":\"18:00:55\",\"trade_timestamp\":1623002455000,"
            + "\"trade_price\":41998000.0,\"trade_volume\":0.00022715,\"ask_bid\":\"BID\","
            + "\"prev_closing_price\":41943000.00000000,\"change\":\"RISE\",\"change_price\":55000.00000000,"
            + "\"sequential_id\":1623002455000000,\"stream_type\":\"REALTIME\"}";

        [Fact]
        void test()
        {
            var sw = Stopwatch.StartNew();
            var numLoop = 100000;
            var opt = new JsonSerializerOptions();
            opt.IncludeFields = true;

            var response = JU.Deserialize<WsResponse>(json);
            var type = response.requestType;

            void run(string name, Action< string> serialzer)
            {
                sw.Restart();
                for (int i = 0; i < numLoop; i++) serialzer(json);
                Debug.WriteLine("{0}: {1}ms", name, sw.ElapsedMilliseconds);
            }

            run("Newton", json => JN.DeserializeObject<WsResponse>(json));
            run("System", json => JS.Deserialize<WsResponse2>(json));
            run("Utf8Json", json => JU.Deserialize<WsResponse>(json));

            run("Newton", json => JN.DeserializeObject<WsResponse>(json));
            run("System", json => JS.Deserialize<WsResponse2>(json));
            run("Utf8Json", json => JU.Deserialize<WsResponse>(json));


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
