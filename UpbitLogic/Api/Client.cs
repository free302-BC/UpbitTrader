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
using System.Text.Json;
using System.Text.Json.Serialization;
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

        static Client()
        {
            #region ---- Candle Impl ~ CandleUnit ----

            _candleTypes = new()
            {
                { CandleUnit.DAY, typeof(CandleDay) },
                { CandleUnit.WEEK, typeof(CandleBase) },
                { CandleUnit.MONTH, typeof(CandleBase) },
            };
            var minutes = Enum.GetValues<CandleUnit>().Where(x => x >= CandleUnit.M1 && x < CandleUnit.DAY).ToArray();
            foreach (var m in minutes) _candleTypes.Add(m, typeof(CandleMinute));

            #endregion

            #region ---- Interface ~ Implimenting Type ----

            addType<IWsRequest>(typeof(WsRequest));
            addType<IWsResponse>(typeof(WsResponse));
            addType<IAccount>(typeof(Account));
            addType<ICandle>(typeof(CandleBase));
            addType<IMarketInfo>(typeof(MarketInfo));
            addType<IOrderbook>(typeof(Orderbook));
            addType<IOrderbookUnit>(typeof(OrderbookUnit));
            addType<ITicker>(typeof(Ticker));
            addType<ITradeTick>(typeof(TradeTick));

            #endregion
        }

        public Client(string accessKey, string secretKey, ILogger logger) :
            base(_wsUri, accessKey, secretKey, logger)
        { }

        protected override void init()
        {
            _wc.SetAuthToken(_key);
            _wc.SetAcceptance();
            _wc.SetAcceptEncoding();
            _ws.Options.KeepAliveInterval = new TimeSpan(0, 1, 30);
        }
        
        /// <summary>
        /// api 호출에 필요한 사전작업 수행 
        /// </summary>
        /// <param name="api"></param>
        /// <param name="postPath"></param>
        /// <returns>http uri</returns>
        protected override string prepareInvoke(ApiId api, string postPath)
        {
            if (Helper.GetApi(api).ResetAuthToken) _wc.SetAuthToken(_key);
            return Helper.GetApiPath(api, postPath);
        }

        protected void setQueryString(string name, CurrencyId currency, CoinId coin)
            => setQueryString(name, Helper.GetMarketId(currency, coin));
        protected void addQueryString(string name, CurrencyId currency, CoinId coin)
            => addQueryString(name, Helper.GetMarketId(currency, coin));


        public IMarketInfo[] ApiMarketInfo() => InvokeApi<MarketInfo>(ApiId.MarketInfoAll).data;

        public string ApiTest(IApiParam param) => param.ToString()!;

        
    }//class
}
