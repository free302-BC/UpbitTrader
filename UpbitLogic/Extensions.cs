using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.Upbit.Model;
using Universe.Utility;
using Universe.CryptoLogic;

namespace Universe.Coin.Upbit
{
    public static class Extensions
    {
        public static List<CalcModel> ApiDayModels(this Client client, int count) 
            => client.ApiCandle<CandleDay>(count).Select(x => x.ToModel()).Reverse().ToList();

        public static CalcModel ToModel(this ICandle candle) => new(candle);

        public static void SetAuthToken(this WebClient wc, KeyPair key) 
            => wc.Headers["Authorization"] = "Bearer " + Helper.BuildAuthToken(key);

        public static void SetAcceptance(this WebClient wc) 
            => wc.Headers["Accept"] = "application/json";

        public static void SetQueryString(this WebClient wc, string key, string value) => wc.QueryString[key] = value;
        public static void RemoveQueryString(this WebClient wc, string key) => wc.QueryString.Remove(key);

        public static void LogWebException(this ILogger logger, WebException ex, ApiId api)
        {
            var nl = Environment.NewLine;
            var msg = $"{api}: uri = { ex.Response?.ResponseUri}{nl}status={ex.Status}: {ex.Message}";
            if (ex.Response != null)
            {
                var buffer = new Span<byte>();
                ex.Response.GetResponseStream().Read(buffer);
                var text = Encoding.ASCII.GetString(buffer);
                msg = $"{msg}{nl}Content= {text}";
            }
            logger.LogError(msg);
        }

        public static string Decode(this string hexa) => Crypto.Decode(getKey(), hexa.FromHexa()).ToUtf8String();
        public static string Encode(this string plain) => Crypto.Encode(getKey(), plain, false).ToHexa();

        static readonly int[] _key =
        {
            172 ^ 0xFF, 158 ^ 0xFF, 186 ^ 0xFF, 29 ^ 0xFF, 129 ^ 0xFF, 117 ^ 0xFF, 73 ^ 0xFF, 069 ^ 0xFF,
            145 ^ 0xFF, 240 ^ 0xFF, 030 ^ 0xFF, 72 ^ 0xFF, 113 ^ 0xFF, 062 ^ 0xFF, 81 ^ 0xFF, 205 ^ 0xFF
        };
        static string getKey() => new Guid(_key.Select(x => (byte)(x ^ 0xFF)).ToArray()).ToString();

    }//class
}
