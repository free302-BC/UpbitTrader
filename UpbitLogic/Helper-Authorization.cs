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

        static byte[] _key = { 172, 158, 186, 29, 129, 117, 73, 69, 145, 240, 30, 72, 113, 62, 81, 205 };
        public static void SaveAuthToken(string accessKey, string secretKey, string filePath)
        {
            var token = buildAuthToken(accessKey, secretKey);
            var bytes = Crypto.Encode(new Guid(_key).ToString(), token, false);
            File.WriteAllBytes(filePath, bytes);
        }
        public static string LoadAuthToken(string filePath)
        {
            var raw = File.ReadAllBytes(filePath);
            return Crypto.DecodeToString(new Guid(_key).ToString(), raw);
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
