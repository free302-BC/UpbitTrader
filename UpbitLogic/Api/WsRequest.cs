using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic;
using Universe.Utility;

namespace Universe.Coin.Upbit
{
    using WsType = Dictionary<string, object>;
    public class WsRequest : IWsRequest
    {
        List<WsType> _value;

        public WsRequest()
        {
            _value = new();
            _value.Add(new() { { "ticket", Guid.NewGuid().ToString() } });
            _value.Add(new() { { "format", "DEFAULT" } });
        }

        public IWsRequest BuildDefault()
        {
            var req = new WsRequest();
            req.Add("orderbook", "KRW-BTC");
            req.Add("trade", "KRW-BTC");
            return req;
        }

        public void Add(string type, string market)
        {
            Add(type, market, ("isOnlyRealtime", true));
        }
        public void Add(string type, string market, (string key, object value) option = default)
        {
            var dic = new WsType();
            dic.Add("type", type);
            dic.Add("codes", new string[] { market });
            
            if (option != default) dic.Add(option.key!, option.value!);
            _value.Add(dic);
        }

        public string ToJson() => JsonSerializer.Serialize(_value);
        public byte[] ToJsonBytes() => ToJson().ToUtf8Bytes();

    }//class

}
