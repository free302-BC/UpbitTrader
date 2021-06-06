using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Universe.Utility;

namespace Universe.Coin.Upbit
{
    using WsType = Dictionary<string, object>;
    public class WsRequest
    {
        List<WsType> _value { get; set; }

        public WsRequest()
        {
            _value = new();
            _value.Add(new() { { "ticket", Guid.NewGuid().ToString() } });
            _value.Add(new() { { "format", "DEFAULT" } });

            Add("trade", "KRW-BTC");
            Add("ticker", "KRW-ETH");
        }
        public void Add(string type, string market)
        {
            var dic = new WsType();
            dic.Add("type", type);
            dic.Add("codes", new string[] { market });
            dic.Add("isOnlyRealtime", true);
            _value.Add(dic);
        }


        public string ToJson() => JsonSerializer.Serialize(_value);
        public byte[] ToJsonBytes() => ToJson().ToUtf8Bytes();

    }//class

}
