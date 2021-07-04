using Microsoft.Extensions.Logging;
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
    using C = TradeTick;
    public partial class Client : ClientBase
    {
        const string _utcFmtTick = "HHmmss";

        public ITradeTick[] ApiTicks(
            CurrencyId currency = CurrencyId.KRW,
            CoinId coin = CoinId.BTC,
            int count = 1,
            DateTime localTo = default)
        {
            clearQueryString();
            setQueryString("market", currency, coin);
            if (localTo != default) setQueryString("to", localTo.ToUniversalTime().ToString(_utcFmtTick));

            var result = new C[count];
            var index = 0;
            _timeCounter.Add();

            while (count > 0)
            {
                setQueryString("count", count.ToString());
                try
                {
                    _timeCounter.Add();
                    var (code, res) = InvokeApi<C>(ApiId.TradeTicks, typeof(C)).Result;//TODO: await or result?

                    if (code == ApiResultCode.Ok)
                    {
                        res.CopyTo(result, index);
                        index += res.Length;
                        count -= res.Length;

                        if (count > 0)
                        {
                            var cursor = res.Last().SequentialId.ToString();
                            setQueryString("cursor", cursor);
                        }
                    }
                    else if (code == ApiResultCode.OkEmpty)
                    {
                        break;
                    }
                    else
                    {
                        _timeCounter.Add();
                    }
                }
                catch (Exception ex)//TODO: 필요한가?
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("----------------------------------------------");
                    sb.AppendLine($"Time: {DateTime.Now:yyMMdd.HHmmss.fff}");
                    sb.AppendLine($"Message: {ex.Message}");
                    sb.AppendLine("----------------------------------------------");
                    _timeCounter.Dump(sb);
                    File.AppendAllText("api_ticks_error.log", sb.ToString());
                    _logger.LogError(sb.ToString());
                    throw;
                }
            }
            if (count == 0) return result;
            return new ArraySegment<C>(result, 0, result.Length - count).ToArray();
        }

    }//class
}
