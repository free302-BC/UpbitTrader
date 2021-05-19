using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit
{
    public static class Extensions
    {
        /// <summary>
        /// Set AuthToken to WebClient
        /// </summary>
        /// <param name="wc"></param>
        /// <param name="key"></param>
        public static void SetAuthToken(this WebClient wc, string token) 
            => wc.Headers.Add("Authorization", "Bearer " + token);

        public static void SetAcceptance(this WebClient wc) 
            => wc.Headers["Accept"] = "application/json";

        public static void SetQueryString(this WebClient wc, NameValueCollection nvc)
        {
            wc.QueryString.Clear();
            wc.QueryString.Add(nvc);
        }

    }//class
}
