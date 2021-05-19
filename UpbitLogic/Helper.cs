using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using IO.Swagger.Api;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Text.Json;
using System.IO;
using System.Text.Unicode;
using System.Text.Encodings.Web;
using Universe.Utility;
using Universe.CryptoLogic;

namespace Universe.Coin.Upbit
{
    using ApiDic = Dictionary<ApiId, (string Path, string Method, string Comment)>;

    public class Helper
    {
        #region ---- API Path Json File ----

        const string _apiBaseUrl = "https://api.upbit.com/v1/";
        const string _apiPathFile = "api_path.json";
        const string _jsonOptionFile = "api_json_option.json";
        static readonly ApiDic _apiDic;
        static Helper()
        {
            var opt = JsonSerializer.Deserialize<JsonSerializerOptions>(File.ReadAllText(_jsonOptionFile));
            _apiDic = JsonSerializer.Deserialize<ApiDic>(File.ReadAllText(_apiPathFile), opt) ?? new ApiDic();
        }
        public static string GetApiUrl(ApiId api) => $"{_apiBaseUrl}{_apiDic[api].Path}";

        static readonly string[] _path =
        {
            "api_keys", "accounts", "status/wallet", "candles/days", "candles/minutes/{unit}",
            "candles/months", "candles/weeks", "deposits/coin_address", "deposits/coin_addresses",
            "deposits/generate_coin_address", "deposit", "deposits", "market/all", "order",
            "orders/chance", "order", "orders", "orders", "orderbook", "ticker", "trades/ticks",
            "withdraws/chance", "withdraws/coin", "withdraw", "withdraws", "withdraws/krw",
        };
        static readonly string[] _method =
        {
            "GET","GET","GET","GET","GET","GET","GET","GET","GET","POST","GET","GET","GET","DELETE",
            "GET","GET","GET","POST","GET","GET","GET","GET","POST","GET","GET","POST"
        };
        static readonly string[] _comment =
        {
            @"API 키 리스트 조회","전체 계좌 조회","입출금 현황","시세 캔들 조회 (일 단위)",
            "시세 캔들 조회 (분 단위)","시세 캔들 조회 (월 단위)","시세 캔들 조회 (주 단위)","개별 입금 주소 조회",
            "전체 입금 주소 조회","입금 주소 생성 요청","개별 입금 조회","입금 리스트 조회","마켓 코드 조회",
            "주문 취소 접수","주문 가능 정보","개별 주문 조회","주문 리스트 조회","주문하기","시세 호가 정보(Orderbook) 조회",
            "시세 Ticker 조회","시세 체결 조회","출금 가능 정보","코인 출금하기","개별 출금 조회","출금 리스트 조회","원화 출금하기"
        };
        public static void BuildApiJson()
        {
            var dic = new ApiDic();
            var len = _path.Length;
            for (int i = 0; i < len; i++) dic.Add((ApiId)i, (_path[i], _method[i], _comment[i]));

            //save json options file
            var optDecoder = new JsonSerializerOptions();
            optDecoder.IncludeFields = true;
            optDecoder.WriteIndented = true;
            optDecoder.PropertyNameCaseInsensitive = true;
            var optJson = JsonSerializer.Serialize(optDecoder, optDecoder);
            File.WriteAllText(_jsonOptionFile, optJson, Encoding.UTF8);

            //save api path file
            var optEncoder = new JsonSerializerOptions(optDecoder);
            optEncoder.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.HangulSyllables);
            var json = JsonSerializer.Serialize(dic, optEncoder);
            File.WriteAllText(_apiPathFile, json, Encoding.UTF8);
        }
        #endregion


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
