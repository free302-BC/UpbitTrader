using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Universe.Coin.Upbit.Model;
using Universe.Utility;

namespace Universe.Coin.Upbit
{
    public partial class Client : ClientBase
    {
        public List<C> ApiCandle<C>(CurrencyId currency = CurrencyId.KRW,
                                    CoinId coin = CoinId.BTC,
                                    int count = 2,
                                    CandleUnit unit = CandleUnit.None)
            where C : ICandle, new()
        {
            ICandle.CheckParam<C>(unit);
            var api = ICandle.GetApiId<C>();
            var postPath = api == ApiId.CandleMinutes ? ((int)unit).ToString() : "";

            return InvokeApi<C>(
                api, 
                postPath: postPath, 
                queryAction: () =>
                {
                    setQueryString("market", currency, coin);
                    setQueryString("count", count.ToString());
                })
            ?? new();
        }

        #region ---- Ticker : 현재시세 조회 ----

        public Ticker ApiTicker(CurrencyId currency = CurrencyId.KRW, CoinId coin = CoinId.BTC)
            => InvokeApi<Ticker>(ApiId.TradeTicker, () => setQueryString("markets", currency, coin))
            ?.FirstOrDefault()
            ?? new();

        public List<Ticker> ApiTicker(IEnumerable<(CurrencyId currency, CoinId coin)> markets)
        {
            return InvokeApi<Ticker>(ApiId.TradeTicker, () =>
            {
                foreach (var q in markets) _wc.QueryString.Add("markets", Helper.GetMarketId(q.currency, q.coin));
            }) ?? new();
        }
        #endregion

    }//class
}
