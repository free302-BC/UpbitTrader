using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit
{
    public enum ApiId
    {
        APIKeyInfo, AccountInfo,
        AccountWallet,
        CandleDays, CandleMinutes, CandleMonth, CandleWeeks,
        DepositCoinAddress, DepositCoinAddresses, DepositGenerateCoinAddress, DepositInfo, DepositInfoAll,
        MarketInfoAll,
        OrderCancel, OrderChance, OrderInfo, OrderInfoAll, OrderNew, OrderOrderbook,
        TradeTicker, TradeTicks,
        WithdrawChance, WithdrawCoin, WithdrawInfo, WithdrawInfoAll, WithdrawKrw
    }

    /// <summary>
    /// 매수시 결제에 사용되는 화폐
    /// </summary>
    public enum CurrencyId { KRW, USDT, BTC }

    public class ApiDic : Dictionary<ApiId, (string Path, string Method, string Comment)> { }
    public class CoinDic : Dictionary<string, (string English, string Korean)> { }
    public class CurrencyDic : Dictionary<CurrencyId, HashSet<string>> { }

    public class KeyPair
    {
        public KeyPair(string a, string s)
        {
            Access = a;
            Secret = s;
        }
        public string Access;
        public string Secret;
        public static implicit operator (string access, string secret)(KeyPair pair) => (pair.Access, pair.Secret);
        public static implicit operator KeyPair((string access, string secret) key) => new KeyPair(key.access, key.secret);
    }


}
