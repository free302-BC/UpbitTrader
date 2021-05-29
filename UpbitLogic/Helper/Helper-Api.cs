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
using System.Diagnostics.CodeAnalysis;
using Universe.Coin.TradeLogic;

namespace Universe.Coin.Upbit
{
    using ApiDic = Dictionary<ApiId, (string Path, HttpMethod Method, string Comment, bool ResetAuthToken)>;
    using _ApiDic = Dictionary<ApiId, (string Path, string Method, string Comment)>;

    public partial class Helper
    {
        const string _apiBaseUrl = "https://api.upbit.com/v1/";
        const string _apiPathFile = "api_path.json";
        const string _jsonOptionFile = "api_json_option.json";
        static readonly ApiDic _apiDic;

        public static string GetApiPath(ApiId api, string postPath = "") 
            => $"{_apiBaseUrl}{_apiDic[api].Path}/{postPath}";

        public static (string Path, HttpMethod Method, string Comment, bool ResetAuthToken) GetApi(ApiId api) 
            => _apiDic[api];

        /// <summary>
        /// AuthKey reset이 필요한 API 목록
        /// </summary>
        static readonly HashSet<ApiId> _resetAuth = new()
        {
            ApiId.AccountInfo,
            ApiId.APIKeyInfo
        };

        #region ---- Build Json ----
        static void buildApiJson()
        {
            var dic = new ApiDic();
            var len = _path.Length;
            for (int i = 0; i < len; i++)
                dic.Add((ApiId)i, (_path[i], _method[i], _comment[i], _resetAuth.Contains((ApiId)i)));

            //save json options file
            var opt = new JsonSerializerOptions();
            opt.IncludeFields = true;
            opt.WriteIndented = true;
            opt.PropertyNameCaseInsensitive = true;
            File.WriteAllText(_jsonOptionFile, JsonSerializer.Serialize(opt, opt), Encoding.UTF8);

            //save api path file
            var optEncoder = new JsonSerializerOptions(opt);
            optEncoder.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.HangulSyllables);
            var json = JsonSerializer.Serialize(dic, optEncoder);
            File.WriteAllText(_apiPathFile, json, Encoding.UTF8);
        }
        static ApiDic loadApiJson(JsonSerializerOptions opt)
        {
            //API url & path
            var json = File.ReadAllText(_apiPathFile);
            return JsonSerializer.Deserialize<_ApiDic>(json, opt)?.ToDictionary(x => x.Key,
                    x => (x.Value.Path, x.Value.Method.To<HttpMethod>(), x.Value.Comment, _resetAuth.Contains(x.Key))) ?? new();
        }

        static readonly string[] _path =
        {
            "api_keys", "accounts", "status/wallet", "candles/days", "candles/minutes",
            "candles/months", "candles/weeks", "deposits/coin_address", "deposits/coin_addresses",
            "deposits/generate_coin_address", "deposit", "deposits", "market/all", "order",
            "orders/chance", "order", "orders", "orders", "orderbook", "ticker", "trades/ticks",
            "withdraws/chance", "withdraws/coin", "withdraw", "withdraws", "withdraws/krw",
        };
        static readonly HttpMethod[] _method =
        {
            HttpMethod.GET, HttpMethod.GET, HttpMethod.GET, HttpMethod.GET, HttpMethod.GET, HttpMethod.GET,
            HttpMethod.GET, HttpMethod.GET, HttpMethod.GET, HttpMethod.POST, HttpMethod.GET, HttpMethod.GET,
            HttpMethod.GET, HttpMethod.DELETE, HttpMethod.GET, HttpMethod.GET, HttpMethod.GET, HttpMethod.POST,
            HttpMethod.GET, HttpMethod.GET, HttpMethod.GET, HttpMethod.GET, HttpMethod.POST, HttpMethod.GET,
            HttpMethod.GET, HttpMethod.POST
        };
        static readonly string[] _comment =
        {
            @"API 키 리스트 조회","전체 계좌 조회","입출금 현황","시세 캔들 조회 (일 단위)",
            "시세 캔들 조회 (분 단위)","시세 캔들 조회 (월 단위)","시세 캔들 조회 (주 단위)","개별 입금 주소 조회",
            "전체 입금 주소 조회","입금 주소 생성 요청","개별 입금 조회","입금 리스트 조회","마켓 코드 조회",
            "주문 취소 접수","주문 가능 정보","개별 주문 조회","주문 리스트 조회","주문하기","시세 호가 정보(Orderbook) 조회",
            "시세 Ticker 조회","시세 체결 조회","출금 가능 정보","코인 출금하기","개별 출금 조회","출금 리스트 조회","원화 출금하기"
        };
        
        #endregion

    }//class
}
