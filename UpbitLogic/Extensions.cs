using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit
{
    public static class Extensions
    {
        /// <summary>
        /// API 호출에 필요한 토큰을 생성
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string BuildAuthToken(this (string AccessKey, string SecretKey) key)
            => Helper.BuildAuthToken(key.AccessKey, key.SecretKey);

        /// <summary>
        /// API 호출에 필요한 토큰을 생성
        /// </summary>
        /// <param name="key"></param>
        /// <param name="nvc"></param>
        /// <returns></returns>
        public static string BuildAuthToken(this (string AccessKey, string SecretKey) key, NameValueCollection nvc)
            => Helper.BuildAuthToken(key.AccessKey, key.SecretKey, nvc);


    }//class
}
