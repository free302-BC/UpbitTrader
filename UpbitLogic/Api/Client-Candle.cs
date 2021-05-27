using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Universe.Coin.Upbit.Model;
using Universe.Utility;

namespace Universe.Coin.Upbit
{
    public partial class Client : ClientBase
    {
        const string _utcFmt = "yyyy-MM-ddTHH:mm:ssZ";
        static long _lastCallTime_Candle = 0;
        static readonly Stopwatch _watch = Stopwatch.StartNew();

        public List<C> ApiCandle<C>(
            CurrencyId currency = CurrencyId.KRW,
            CoinId coin         = CoinId.BTC,
            int count           = 2,
            CandleUnit unit     = CandleUnit.None,
            DateTime localTo    = default)
            where C : ICandle, new()
        {
            ICandle.CheckParam<C>(unit);
            var api = ICandle.GetApiId<C>();
            var postPath = api == ApiId.CandleMinutes ? ((int)unit).ToString() : "";

            var result = new List<C>();
            clearQueryString();
            setQueryString("market", currency, coin);
            if (localTo != default) setQueryString("to", $"{localTo.ToUniversalTime():_utcFmt}");

            while (count > 0)
            {
                setQueryString("count", count.ToString());

                var res = InvokeApi<C>(api, postPath);
                if (res.Count > 0)
                {
                    count -= res.Count;
                    if (count > 0)
                    {
                        var to = DateTimeOffset.FromUnixTimeMilliseconds(res.Last().Timestamp).UtcDateTime;
                        setQueryString("to", to.ToString(_utcFmt));
                    }
                }

                var dt = _watch.ElapsedMilliseconds - _lastCallTime_Candle;
                if (dt < 100) Thread.Sleep((int)(100 - dt));
                _lastCallTime_Candle = _watch.ElapsedMilliseconds;
            }

            return result;//.OrderBy(x => x.Timestamp).ToList();
        }

        #region ---- Ticker : 현재시세 조회 ----

        public Ticker ApiTicker(CurrencyId currency = CurrencyId.KRW, CoinId coin = CoinId.BTC)
        {
            clearQueryString();
            setQueryString("markets", currency, coin);
            return InvokeApi<Ticker>(ApiId.TradeTicker)?.FirstOrDefault() ?? new();
        }

        public List<Ticker> ApiTicker(IEnumerable<(CurrencyId currency, CoinId coin)> markets)
        {
            clearQueryString();
            foreach (var q in markets) _wc.QueryString.Add("markets", Helper.GetMarketId(q.currency, q.coin));
            return InvokeApi<Ticker>(ApiId.TradeTicker) ?? new();
        }
        #endregion

    }//class
}
