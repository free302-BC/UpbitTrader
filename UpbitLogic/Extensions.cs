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

namespace Universe.Coin.Upbit
{
    public static class Extensions
    {
        public static List<CalcModel> ApiDayModels(this Client client, int count) 
            => client.ApiCandle<CandleDay>(count).Select(x => x.ToModel()).Reverse().ToList();

        public static CalcModel ToModel(this ICandle candle) => new(candle);

        /// <summary>
        /// Set AuthToken to WebClient
        /// </summary>
        /// <param name="wc"></param>
        /// <param name="key"></param>
        public static void SetAuthToken(this WebClient wc, string tokenFileName) 
            => wc.Headers.Add("Authorization", "Bearer " + Helper.LoadAuthToken(tokenFileName));

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

    }//class
}
