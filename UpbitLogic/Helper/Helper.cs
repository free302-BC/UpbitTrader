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
    public partial class Helper
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        static Helper()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            try
            {
                var opt = JsonSerializer.Deserialize<JsonSerializerOptions>(File.ReadAllText(_jsonOptionFile)) ?? new();
                _apiDic = loadApiJson(opt)!;//API url & path
                _coinNames = loadCoinJson(opt)!;//coin name dic
                _currencyCoins = loadCurrencyJson(opt)!;//currency-coin dic -> market
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Execption from {nameof(Helper)}.ctor():");
                Console.WriteLine(ex);
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine($"Execption from {nameof(Helper)}.ctor():");
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Execption from {nameof(Helper)}.ctor():");
                Console.WriteLine(ex);
            }
        }

    }//class
}
