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
    using ApiDic = Dictionary<ApiId, (string Path, string Method, string Comment)>;
    using CoinDic = Dictionary<CoinId, (string English, string Korean)>;
    using CurrencyDic = Dictionary<CurrencyId, HashSet<CoinId>>;

    public partial class Helper
    {
        #region ---- API Path Json File ----

        const string _apiBaseUrl = "https://api.upbit.com/v1/";
        const string _apiPathFile = "api_path.json";
        const string _jsonOptionFile = "api_json_option.json";
        static readonly ApiDic _apiDic;
        
        public static string GetApiUrl(ApiId api) => $"{_apiBaseUrl}{_apiDic[api].Path}";
        public static void BuildApiJson()
        {
            var dic = new ApiDic();
            var len = _path.Length;
            for (int i = 0; i < len; i++) dic.Add((ApiId)i, (_path[i], _method[i], _comment[i]));

            //save json options file
            var opt = new JsonSerializerOptions();
            opt.IncludeFields = true;
            opt.WriteIndented = true;
            opt.PropertyNameCaseInsensitive = true;
            var optJson = JsonSerializer.Serialize(opt, opt);
            File.WriteAllText(_jsonOptionFile, optJson, Encoding.UTF8);

            //save api path file
            var optEncoder = new JsonSerializerOptions(opt);
            optEncoder.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.HangulSyllables);
            var json = JsonSerializer.Serialize(dic, optEncoder);
            File.WriteAllText(_apiPathFile, json, Encoding.UTF8);
        }

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
        
        #endregion

    }//class
}
