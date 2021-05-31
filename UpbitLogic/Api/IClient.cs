using System;
using System.Collections.Generic;
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Model;
using Universe.Coin.Upbit.Model;

namespace Universe.Coin.Upbit
{
    public interface IClient
    {
        Account[] ApiAccount();
        C[] ApiCandle<C>(CurrencyId currency = CurrencyId.KRW, CoinId coin = CoinId.BTC, CandleUnit unit = CandleUnit.DAY, int count = 2, DateTime localTo = default) where C : ICandle, new();
        MarketInfo[] ApiMarketInfo();
        Orderbook ApiOrderbook(CurrencyId currency = CurrencyId.KRW, CoinId coin = CoinId.BTC);
        Ticker ApiTicker(CurrencyId currency = CurrencyId.KRW, CoinId coin = CoinId.BTC);
        Ticker[] ApiTicker(IEnumerable<(CurrencyId currency, CoinId coin)> markets);
        TradeTick[] ApiTicks(CurrencyId currency = CurrencyId.KRW, CoinId coin = CoinId.BTC, int count = 1);
        decimal GetBalance(CoinId coin);
        decimal GetBalance(CurrencyId currency);

        /// <summary>
        /// TEST...
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        string ApiTest(IApiParam param);

    }
}