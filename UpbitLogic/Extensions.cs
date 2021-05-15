using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit
{
    public static class Extensions
    {
        
        public static SigningCredentials Sign(this string secretKey)
        {
            byte[] keyBytes = Encoding.Default.GetBytes(secretKey);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKey, "HS256");
            return credentials;
        }
        public static string BuildJwtToken(this SigningCredentials credentials, string accessKey)
        {
            var payload = new JwtPayload
            {
                { "access_key", accessKey },
                { "nonce", Guid.NewGuid().ToString() },
                //{ "query_hash", queryHash },
                //{ "query_hash_alg", "SHA512" }
            };

            var header = new JwtHeader(credentials);            
            var secToken = new JwtSecurityToken(header, payload);
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(secToken);
            return jwtToken;
        }


        public static string BuildAuthToken(this WorkerSetting set) => Helper.BuildAuthToken(keys.Item1, keys.Item2);



    }//class
}
