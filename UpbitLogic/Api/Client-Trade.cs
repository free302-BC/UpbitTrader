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
        public Orderbook ApiOrderbook(CurrencyId currency = CurrencyId.KRW, CoinId coin = CoinId.BTC)
        {
            var res = InvokeApi<Orderbook>(ApiId.OrderOrderbook, () => setQueryString("markets", currency, coin))
                ?.FirstOrDefault()
                ?? new();
            return res;
        }
        public List<TradeTick> ApiTicks(CurrencyId currency = CurrencyId.KRW, CoinId coin = CoinId.BTC, int count = 1)
        {
            return InvokeApi<TradeTick>(ApiId.TradeTicks, () =>
            {
                setQueryString("market", currency, coin);
                setQueryString("count", count);
            });
        }

    }//class
}
