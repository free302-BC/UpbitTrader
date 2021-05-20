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

namespace Universe.Coin.Upbit
{
    public partial class Helper
    {
        #region ---- Authorization Token (Payload) ----

        static readonly int[] _key =
        {
            172 ^ 0xFF, 158 ^ 0xFF, 186 ^ 0xFF, 29 ^ 0xFF, 129 ^ 0xFF, 117 ^ 0xFF, 73 ^ 0xFF, 069 ^ 0xFF,
            145 ^ 0xFF, 240 ^ 0xFF, 030 ^ 0xFF, 72 ^ 0xFF, 113 ^ 0xFF, 062 ^ 0xFF, 81 ^ 0xFF, 205 ^ 0xFF
        };
        public static void SaveAuthToken(string accessKey, string secretKey, string filePath)
        {
            var token = buildAuthToken(accessKey, secretKey);
            var key = new Guid(_key.Select(x => (byte)(x ^ 0xFF)).ToArray()).ToString();
            var bytes = Crypto.Encode(key, token, false);
            File.WriteAllBytes(filePath, bytes);
        }
        public static string LoadAuthToken(string filePath)
        {
            var raw = File.ReadAllBytes(filePath);
            var key = new Guid(_key.Select(x => (byte)(x ^ 0xFF)).ToArray()).ToString();
            return Crypto.DecodeToString(key, raw);
        }

        #region ---- Build Auth Token ----

        static string buildAuthToken(string accessKey, string secretKey)
        {
            var sign = Helper.sign(secretKey);
            var payload = buildPayload(accessKey);
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

        #endregion


    }//class
}
