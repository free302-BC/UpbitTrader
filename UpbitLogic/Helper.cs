using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.IO;
using System.Text.Unicode;
using System.Text.Encodings.Web;
using Universe.Utility;
using Universe.CryptoLogic;
using System.Text.Json;

namespace Universe.Coin.Upbit
{
    using ApiDic = Dictionary<ApiId, (string Path, string Method, string Comment)>;
    using CoinDic = Dictionary<CoinId, (string English, string Korean)>;
    using CurrencyDic = Dictionary<CurrencyId, HashSet<CoinId>>;

    public partial class Helper
    {
        static Helper()
        {
            var opt = JsonSerializer.Deserialize<JsonSerializerOptions>(File.ReadAllText(_jsonOptionFile));

            //API url & path
            var json = File.ReadAllText(_apiPathFile);
            _apiDic = JsonSerializer.Deserialize<ApiDic>(json, opt) ?? new ApiDic();

            //coin name dic
            json = File.ReadAllText(_CoinNameFile);
            _coinNames = JsonSerializer.Deserialize<CoinDic>(json, opt) ?? new CoinDic();

            //currency-coin dic -> market
            json = File.ReadAllText(_MarketCoinsFile);
            //_marketCoins = JsonSerializer.Deserialize<CurrencyDic>(json, opt) ?? new CurrencyDic();
            var markets = JsonSerializer.Deserialize<Dictionary<CurrencyId, HashSet<string>>>(json, opt)
                ?? new Dictionary<CurrencyId, HashSet<string>>();
            _marketCoins = markets.ToDictionary(x => x.Key, x => x.Value.Select(y => y.To<CoinId>()).ToHashSet());
        }

    }//class
}
