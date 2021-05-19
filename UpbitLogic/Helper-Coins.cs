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
    using CoinDic = Dictionary<string, (string English, string Korean)>;
    using CurrencyDic = Dictionary<CurrencyId, string[]>;

    public partial class Helper
    {

        #region ---- Market/Coin Info ----

        const string _CoinNameFile = "coin-name.json";
        const string _MarketCoinsFile = "market-coins.json";
        public static Dictionary<string, (string English, string Korean)> CoinNames;
        public static Dictionary<CurrencyId, string[]> MarketCoins;
        public static void Load()
        {
            var json = File.ReadAllText(_CoinNameFile);
            CoinNames = JsonSerializer.Deserialize<Dictionary<string, (string English, string Korean)>>(json);

            json = File.ReadAllText(_MarketCoinsFile);
            MarketCoins = JsonSerializer.Deserialize<Dictionary<CurrencyId, string[]>>(json);
        }

        #endregion


    }//class
}
