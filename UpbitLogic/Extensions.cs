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
        /// BuildAuthToken
        /// </summary>
        /// <param name="wc"></param>
        /// <param name="key"></param>
        public static void BuildAuthToken(this WebClient wc, (string AccessKey, string SecretKey) key)
            => wc.Headers.Add("Authorization", Helper.BuildAuthToken(key.AccessKey, key.SecretKey));

        public static void BuildAuthToken(this WebClient wc, (string AccessKey, string SecretKey) key, NameValueCollection nvc)
            => wc.Headers.Add("Authorization", Helper.BuildAuthToken(key.AccessKey, key.SecretKey, nvc));


    }//class
}
