﻿using Microsoft.Extensions.Logging;
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
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Model;
using Universe.Coin.Upbit.Model;
using Universe.Utility;

namespace Universe.Coin.Upbit
{
    public partial class Client : ClientBase
    {
        const string _utcFmt = "yyyy-MM-ddTHH:mm:ssZ";
        static long _lastCallTime;
        static readonly Stopwatch _watch;

        //-------------------- TEST ------------------------------
        public static List<(CandleUnit, long, long)> CallTimes = new();
        static Client()
        {
            _watch =  Stopwatch.StartNew();
            _lastCallTime = 0;
        }
        //--------------------------------------------------------

        public C[] ApiCandle<C>(
            CurrencyId currency = CurrencyId.KRW,
            CoinId coin         = CoinId.BTC,
            CandleUnit unit     = CandleUnit.DAY,
            int count           = 2,
            DateTime localTo    = default)
            where C : ICandle, new()
        {
            var api = ICandle.GetApiId(unit);
            var postPath = api == ApiId.CandleMinutes ? ((int)unit).ToString() : "";

            clearQueryString();
            setQueryString("market", currency, coin);
            if (localTo != default) setQueryString("to", $"{localTo.ToUniversalTime():_utcFmt}");

            var result = new C[count];
            var index = 0;

            while (count > 0)
            {
                setQueryString("count", count.ToString());

                try
                {                    
                    var res = InvokeApi<C>(api, postPath);

                    if (res.Length > 0)
                    {
                        res.CopyTo(result, index);
                        index += res.Length;
                        count -= res.Length;

                        if (count > 0)
                        {
                            var to = DateTimeOffset.FromUnixTimeMilliseconds(res.Last().Timestamp).UtcDateTime;
                            setQueryString("to", to.ToString(_utcFmt));
                        }
                    }
                    //if (count > 0)
                    {
                        var dt = _watch.ElapsedMilliseconds - _lastCallTime;
                        CallTimes.Add((unit, _watch.ElapsedMilliseconds, dt));
                        //if (dt < 100 && _lastCallTime != 0)
                        //    throw new Exception($"dt<100: {unit} dt={dt} @{_watch.ElapsedMilliseconds}");

                        if (dt < 100)
                        {
                            CallTimes.Add((CandleUnit.DAY, _watch.ElapsedMilliseconds, - 100 + dt));
                            Thread.Sleep((int)(100 - dt));
                        }
                        _lastCallTime = _watch.ElapsedMilliseconds;
                    }
                }
                catch (Exception ex)
                {
                    CallTimes.Add((CandleUnit.None, _watch.ElapsedMilliseconds, _watch.ElapsedMilliseconds - _lastCallTime));
                    ex.Data.Add("ms", CallTimes);
                    throw;
                }
            }

            //if(index != result.Length) Array.Resize(ref result, index);
            return result;//.OrderBy(x => x.Timestamp).ToList();
        }

        #region ---- Ticker : 현재시세 조회 ----

        public Ticker ApiTicker(CurrencyId currency = CurrencyId.KRW, CoinId coin = CoinId.BTC)
        {
            clearQueryString();
            setQueryString("markets", currency, coin);
            return InvokeApi<Ticker>(ApiId.TradeTicker)?.FirstOrDefault() ?? new();
        }

        public Ticker[] ApiTicker(IEnumerable<(CurrencyId currency, CoinId coin)> markets)
        {
            clearQueryString();
            foreach (var q in markets) _wc.QueryString.Add("markets", Helper.GetMarketId(q.currency, q.coin));
            return InvokeApi<Ticker>(ApiId.TradeTicker) ?? Array.Empty<Ticker>();
        }
        #endregion

    }//class
}
