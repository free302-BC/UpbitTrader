﻿using Microsoft.Extensions.Logging;
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
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Model;
using Universe.Coin.Upbit.Model;
using Universe.Utility;

namespace Universe.Coin.Upbit
{
    public partial class Client : ClientBase
    {
        public IOrderbook ApiOrderbook(CurrencyId currency = CurrencyId.KRW, CoinId coin = CoinId.BTC)
        {
            clearQueryString();
            setQueryString("markets", currency, coin);

            return InvokeApi<Orderbook>(ApiId.OrderOrderbook).Cast<IOrderbook>()?.FirstOrDefault() 
                ?? new Orderbook();

            //return InvokeApi<Orderbook>(ApiId.OrderOrderbook)?.FirstOrDefault()?? new();
        }
        public ITradeTick[] ApiTicks(CurrencyId currency = CurrencyId.KRW, CoinId coin = CoinId.BTC, int count = 1)
        {
            clearQueryString();
            setQueryString("market", currency, coin);
            setQueryString("count", count);
            return InvokeApi<TradeTick>(ApiId.TradeTicks);
        }

    }//class
}
