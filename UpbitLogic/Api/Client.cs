using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Model;
using Universe.Coin.Upbit.Model;
using Universe.Utility;

namespace Universe.Coin.Upbit
{
    public partial class Client : ClientBase, IClient
    {
        const string _wsUri = "wss://api.upbit.com/websocket/v1";

        public Client(string accessKey, string secretKey, ILogger logger) :
            base(_wsUri, accessKey, secretKey, logger)
        { }

        protected override void init()
        {
            _wc.SetAuthToken(_key);
            _wc.SetAcceptance();
            _ws.Options.KeepAliveInterval = new TimeSpan(0, 1, 30);
        }        
        protected override string prepareInvoke(ApiId api, string postPath)
        {
            if (Helper.GetApi(api).ResetAuthToken) _wc.SetAuthToken(_key);
            return Helper.GetApiPath(api, postPath);
        }

        protected void setQueryString(string name, CurrencyId currency, CoinId coin)
            => setQueryString(name, Helper.GetMarketId(currency, coin));
        protected void addQueryString(string name, CurrencyId currency, CoinId coin)
            => addQueryString(name, Helper.GetMarketId(currency, coin));


        public IMarketInfo[] ApiMarketInfo() => InvokeApi<MarketInfo>(ApiId.MarketInfoAll);

        public string ApiTest(IApiParam param) => param.ToString()!;

        
    }//class
}
