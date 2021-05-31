using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic;

namespace Universe.Coin.Upbit
{
    public class ApiParam : IApiParam
    {
        public CurrencyId currency { get; } = CurrencyId.KRW;
        public CoinId coin { get; } = CoinId.BTC;
        public ApiId api { get; } = ApiId.CandleDays;
        public CandleUnit unit { get; } = CandleUnit.DAY;
        public int count { get; } = 2;
        public DateTime localTo { get; } = DateTime.Now;

        public override string ToString() => IApiParam.ToString(this);

    }
}
