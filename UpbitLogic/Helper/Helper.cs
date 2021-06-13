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
using System.Text.Json.Serialization;

namespace Universe.Coin.Upbit
{
    public partial class Helper
    {
#pragma warning disable CS8618
        static Helper()
        {
            try
            {
                _jsonOption = getJsonOptions();

                _apiDic = loadApiJson(_jsonOption);//API url & path
                _coinNames = loadCoinJson(_jsonOption);//coin name dic
                _currencyCoins = loadCurrencyJson(_jsonOption);//currency-coin dic -> market
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

        static JsonSerializerOptions _jsonOption;
        static JsonSerializerOptions getJsonOptions()
        {
            var opt = new JsonSerializerOptions();
            opt.IncludeFields = true;
            opt.WriteIndented = true;
            opt.PropertyNameCaseInsensitive = false;
            opt.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            opt.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.HangulSyllables);
            return opt;
        }

    }//class
#pragma warning restore CS8618

}
