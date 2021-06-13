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
    using OptPair = ValueTuple<string, object>;

    public class WsRequest : IWsRequest
    {
        static readonly OptPair _opt = ("isOnlyRealtime", true);
        const string _market = "KRW-BTC";
        readonly List<WsType> _value;

        public WsRequest()
        {
            _value = new();
            _value.Add(new() { { "ticket", Guid.NewGuid().ToString() } });
            _value.Add(new() { { "format", "DEFAULT" } });
        }

        public IWsRequest ToDefault()
        {
            if(_value.Count > 2) _value.RemoveRange(2, _value.Count);
            AddTrade();
            AddOrderbook();
            AddTicker();
            return this;
        }

        public void AddTrade(CurrencyId currency = CurrencyId.KRW, CoinId coin = CoinId.BTC, OptPair option = default) 
            => add("trade", currency, coin, option);
        public void AddOrderbook(CurrencyId currency = CurrencyId.KRW, CoinId coin = CoinId.BTC, OptPair option = default) 
            => add("orderbook", currency, coin, option);
        public void AddTicker(CurrencyId currency = CurrencyId.KRW, CoinId coin = CoinId.BTC, OptPair option = default) 
            => add("ticker", currency, coin, option);

        void add(string type, CurrencyId currency = CurrencyId.KRW, CoinId coin = CoinId.BTC, 
            (string key, object value) option = default)
        {
            var dic = new WsType();
            dic.Add("type", type);
            dic.Add("codes", new string[] { Helper.GetMarketId(currency, coin) });

            if (option != default && option.key != null) dic.Add(option.key, option.value ?? "");
            else dic.Add(_opt.Item1, _opt.Item2);
            _value.Add(dic);
        }

        public string ToJson() => JsonSerializer.Serialize(_value);
        public byte[] ToJsonBytes() => ToJson().ToUtf8Bytes();
        
    }//class
}
