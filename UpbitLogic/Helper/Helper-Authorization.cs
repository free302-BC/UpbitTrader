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
using Universe.Coin.TradeLogic;

namespace Universe.Coin.Upbit
{
    public partial class Helper
    {
        #region ---- Build Auth Token ----

        public static string BuildAuthToken(KeyPair key)
        {
            var sign = Helper.sign(key.Secret);
            var payload = buildPayload(key.Access);
            var token = buidlJwtToken(sign, payload);
            return token;
        }
        static SigningCredentials sign(string secretKey)
        {
            var bytes = Encoding.Default.GetBytes(secretKey);
            var securityKey = new SymmetricSecurityKey(bytes);
            return new SigningCredentials(securityKey, "HS256");
        }

        static string buidlJwtToken(SigningCredentials credentials, JwtPayload payload)
        {
            var header = new JwtHeader(credentials);
            var secToken = new JwtSecurityToken(header, payload);
            return new JwtSecurityTokenHandler().WriteToken(secToken);
        }
        #endregion

        #region ---- Payload ----

        static JwtPayload buildPayload(string accessKey)
        {
            return new JwtPayload
            {
                { "access_key", accessKey },
                { "nonce", Guid.NewGuid().ToString() }
            };
        }
        static JwtPayload buildPayload(string accessKey, string queryHash)
        {
            return new JwtPayload
            {
                { "access_key", accessKey },
                { "nonce", Guid.NewGuid().ToString() },
                { "query_hash", queryHash },
                { "query_hash_alg", "SHA512" }
            };
        }
        #endregion

        #region ---- Payload with QueryString (필요?) ----
        internal static string BuildAuthToken(string accessKey, string secretKey, NameValueCollection nvc)
        {
            var sing = sign(secretKey);
            var payload = buildPayload(accessKey, buildQueryHash(nvc));
            var token = buidlJwtToken(sing, payload);
            return token;
        }
        public static string buildQueryHash(NameValueCollection nvc) => $"{nvc}".ToHashString();
        #endregion

        #region ---- TEST ----

        public static void SaveEncrptedKey(string accessKey, string secretKey, string filePath)
        {
            var ak = accessKey.Encode();
            var sk = secretKey.Encode();
            File.WriteAllText(filePath, $"{ak}\n{sk}");
        } 
        #endregion

    }//class
}
