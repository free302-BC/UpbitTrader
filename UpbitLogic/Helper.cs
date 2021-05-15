using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit
{
    public class Helper
    {
        public static string BuildAuthToken(string accessKey, string secretKey)
        {
            // parameter가 Dictionary<string, string> 일 경우
            //StringBuilder builder = new StringBuilder();
            //foreach (KeyValuePair<string, string> pair in parameters)
            //{
            //    builder.Append(pair.Key).Append("=").Append(pair.Value).Append("&");
            //}
            //string queryString = builder.ToString().TrimEnd('&');

            //SHA512 sha512 = SHA512.Create();
            //byte[] queryHashByteArray = sha512.ComputeHash(Encoding.UTF8.GetBytes(queryString));
            //string queryHash = BitConverter.ToString(queryHashByteArray).Replace("-", "").ToLower();


            var credentials = secretKey.Sign();
            string jwtToken = credentials.BuildJwtToken(accessKey);
            return jwtToken;
            //var authorizationToken = "Bearer " + jwtToken;
            //return authorizationToken;
        }

        
    }//class
}
