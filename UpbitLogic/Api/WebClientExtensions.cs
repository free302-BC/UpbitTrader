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

namespace Universe.Coin.Upbit
{
    public static class WebClientExtensions
    {
        public static void SetAuthToken(this WebClient wc, KeyPair key)
            => wc.Headers["Authorization"] = "Bearer " + Helper.BuildAuthToken(key);

        public static void SetAcceptance(this WebClient wc)
            => wc.Headers["Accept"] = "application/json";

        public static void SetAcceptEncoding(this WebClient wc)
                => wc.Headers["Accept-Encoding"] = "gzip";

        public static void ClearAcceptEncoding(this WebClient wc)
                => wc.Headers.Remove("Accept-Encoding");

    }//class
}
