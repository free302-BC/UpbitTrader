﻿using Microsoft.Extensions.Logging;
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
        static readonly TimeCounter _timeCounter = new(1000, 10);

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
                    _timeCounter.Add();
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
                }
                catch (Exception ex)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("----------------------------------------------");
                    sb.AppendLine($"Time: {DateTime.Now:yyMMdd.HHmmss.fff}");
                    sb.AppendLine($"Message: {ex.Message}");
                    sb.AppendLine("----------------------------------------------");
                    _timeCounter.Dump(sb);
                    File.AppendAllText("candle_time_counter.txt", sb.ToString());
                }
            }

            //if(index != result.Length) Array.Resize(ref result, index);
            return result;//.OrderBy(x => x.Timestamp).ToList();
        }

        #region ---- Ticker : 현재시세 조회 ----

        public ITicker ApiTicker(CurrencyId currency = CurrencyId.KRW, CoinId coin = CoinId.BTC)
        {
            clearQueryString();
            setQueryString("markets", currency, coin);
            return InvokeApi<Ticker>(ApiId.TradeTicker)?.FirstOrDefault() ?? new();
        }

        public ITicker[] ApiTicker(IEnumerable<(CurrencyId currency, CoinId coin)> markets)
        {
            clearQueryString();
            foreach (var q in markets) addQueryString("markets", q.currency, q.coin);
            return InvokeApi<Ticker>(ApiId.TradeTicker) ?? Array.Empty<Ticker>();
        }
        #endregion

    }//class
}
