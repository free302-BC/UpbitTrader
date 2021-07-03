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
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Model;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Universe.Coin.Upbit
{
    public static class WebClientExtensions
    {
        public static void SetAuthToken(this WebClient wc, string authToken)
            => wc.Headers["Authorization"] = "Bearer " + authToken;// Helper.BuildAuthToken(key);

        public static void SetAcceptance(this WebClient wc)
            => wc.Headers["Accept"] = "application/json";

        public static void SetAcceptEncoding(this WebClient wc)
                => wc.Headers["Accept-Encoding"] = "gzip";

        public static void ClearAcceptEncoding(this WebClient wc)
                => wc.Headers.Remove("Accept-Encoding");

    }//class

    public static class HttpClientExtensions
    {
        public static void SetAuthToken(this HttpClient wc, string authToken)
        {
            wc.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("Bearer", authToken);
            //wc.Headers["Authorization"] = "Bearer " + Helper.BuildAuthToken(key);
        }

        public static void SetAcceptance(this HttpClient wc)
        {
            wc.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //wc.Headers["Accept"] = "application/json";
        }

        public static void SetAcceptEncoding(this HttpClient wc)
        {
            wc.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            //wc.Headers["Accept-Encoding"] = "gzip";
        }

        public static void ClearAcceptEncoding(this HttpClient wc)
        {
            wc.DefaultRequestHeaders.AcceptEncoding.Clear();
            //wc.Headers.Remove("Accept-Encoding");
        }

    }//class
}
