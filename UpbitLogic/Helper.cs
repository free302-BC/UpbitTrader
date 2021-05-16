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

namespace Universe.Coin.Upbit
{
    using ApiDic = Dictionary<Api, (string Path, string Method, string Comment)>;

    public class Helper
    {
        const string _apiBaseUrl = "https://api.upbit.com/v1/";
        const string _apiPathFile = "api_path.json";
        const string _jsonOptionFile = "api_json_option.json";

        static readonly ApiDic _apiPath;
        static Helper()
        {
            var opt = JsonSerializer.Deserialize<JsonSerializerOptions>(File.ReadAllText(_jsonOptionFile));
            _apiPath = JsonSerializer.Deserialize<ApiDic>(File.ReadAllText(_apiPathFile), opt) ?? new ApiDic();
        }
        public static string GetApiUrl(Api api) => $"{_apiBaseUrl}{_apiPath[api].Path}";


        #region ---- Build API Path Json File ----
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
        public static void build()
        {
            var dic = new ApiDic();
            var len = _path.Length;
            for (int i = 0; i < len; i++) dic.Add((Api)i, (_path[i], _method[i], _comment[i]));

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


        #region ---- Authorization Token (inc. Payload) ----
        public static string BuildAuthToken(string accessKey, string secretKey)
        {
            var sing = sign(secretKey);
            var payload = buildPayload(accessKey);
            var token = buidlJwtToken(sing, payload);
            return $"Bearer {token}";
        }
        public static string BuildAuthToken(string accessKey, string secretKey, NameValueCollection nvc)
        {
            var sing = sign(secretKey);
            var payload = buildPayload(accessKey, buildQueryHash(nvc));
            var token = buidlJwtToken(sing, payload);
            return $"Bearer {token}";
        }

        internal static string buildQueryHash(NameValueCollection nvc)
        {
            using var sha512 = SHA512.Create();
            byte[] queryHashByteArray = sha512.ComputeHash(Encoding.UTF8.GetBytes($"{nvc}"));
            string queryHash = BitConverter.ToString(queryHashByteArray).Replace("-", "");//.ToLower()
            return queryHash;
        }

        internal static SigningCredentials sign(string secretKey)
        {
            var bytes = Encoding.Default.GetBytes(secretKey);
            var securityKey = new SymmetricSecurityKey(bytes);
            return new SigningCredentials(securityKey, "HS256");
        }

        internal static string buidlJwtToken(SigningCredentials credentials, JwtPayload payload)
        {
            var header = new JwtHeader(credentials);
            var secToken = new JwtSecurityToken(header, payload);
            return new JwtSecurityTokenHandler().WriteToken(secToken);
        }

        internal static JwtPayload buildPayload(string accessKey)
        {
            return new JwtPayload
            {
                { "access_key", accessKey },
                { "nonce", Guid.NewGuid().ToString() }
            };
        }
        internal static JwtPayload buildPayload(string accessKey, string queryHash)
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



    }//class
}
