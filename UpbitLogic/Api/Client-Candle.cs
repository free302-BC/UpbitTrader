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
        public List<M> ApiCandle<M>(CurrencyId currency = CurrencyId.KRW, CoinId coin = CoinId.BTC, int count = 15)
            where M : class, new()
        {
            //TODO: decide apiID from M or from param
            //TODO: make candle BaseModel
            return InvokeApi<M>(ApiId.CandleDays, () =>
                {
                    setQueryString("market", currency, coin);
                    setQueryString("count", count.ToString());
                })
            ?? new();
        }

        public Ticker ApiTicker(CurrencyId currency = CurrencyId.KRW, CoinId coin = CoinId.BTC)
            => InvokeApi<Ticker>(ApiId.TradeTicker, () => setQueryString("markets", currency, coin))
            ?.FirstOrDefault()
            ?? new();

        public List<Ticker> ApiTicker(IEnumerable<(CurrencyId currency, CoinId coin)> markets)
        {
            void setQs()
            {
                foreach (var q in markets) _wc.QueryString.Add("markets", Helper.GetMarketId(q.currency, q.coin));
            }
            return InvokeApi<Ticker>(ApiId.TradeTicker, setQs) ?? new();
        }
    }//class
}
