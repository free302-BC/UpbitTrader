using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic;

namespace Universe.Coin.Upbit
{
    public interface IApiParam
    {
        CurrencyId currency { get; }
        CoinId coin { get; }
        ApiId api { get; }
        CandleUnit unit { get; }
        int count { get; }
        DateTime localTo { get; }
        static string ToString(IApiParam p)
        {
            var sb = new StringBuilder()
                .AppendLine($"{nameof(currency)}: {p.currency,16}")
                .AppendLine($"{nameof(coin)}: {p.coin,16}")
                .AppendLine($"{nameof(api)}: {p.api,16}")
                .AppendLine($"{nameof(unit)}: {p.unit,16}")
                .AppendLine($"{nameof(count)}: {p.count,16}")
                .AppendLine($"{nameof(localTo)}: {p.localTo,16}");
            return sb.ToString();
        }
    }
}
