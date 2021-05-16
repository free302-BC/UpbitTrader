using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace Universe.Coin.Upbit
{
    using ResJson = List<Dictionary<string, object>>;

    public class UpbitClient : IDisposable
    {
        ILogger _logger;
        WebClient _wc;
        (string AccessKey, string SecretKey) _key;

        public UpbitClient((string AccessKey, string SecretKey) key, ILogger logger)
        {
            _logger = logger;
            _wc = new WebClient();
            _wc.BuildAuthToken(key);
            _wc.Headers["Accept"] = "application/json";
        }
        public void Dispose()
        {
            _wc?.Dispose();
        }

        public void AddQuery(NameValueCollection nvc) => _wc.QueryString.Add(nvc);

        public ResJson ApiCandleDay()
        {
            var api = Api.CandleDays;
            var nvc = HttpUtility.ParseQueryString(string.Empty);
            nvc.Add("market", "KRW-BTC");// string | 마켓 코드 (ex. KRW-BTC) 
            nvc.Add("to", "");// string | 마지막 캔들 시각 (exclusive). 포맷 : `yyyy-MM-dd'T'HH:mm:ssXXX` or `yyyy-MM-dd HH:mm:ss`. 비워서 요청 시 가장 최근 캔들  (optional) 
            nvc.Add("count", "8");  // decimal? | 캔들 개수  (optional) 
            nvc.Add("convertingPriceUnit", "KRW");

            _wc.QueryString.Clear();
            _wc.QueryString.Add(nvc);
            //_wc.BuildAuthToken(_key);

            try
            {
                var response = _wc.DownloadString(Helper.GetApiUrl(api));
                var list = JsonSerializer.Deserialize<ResJson>(response) ?? new ResJson();
                return list;
            }
            catch (WebException ex)
            {
                var msg = $"API={api}: uri = { ex.Response?.ResponseUri}\nstatus={ex.Status}: {ex.Message}";
                if (ex.Response != null)
                {
                    var stream = ex.Response.GetResponseStream();
                    var sr = new StreamReader(stream);
                    var text = sr.ReadToEnd();
                    msg = $"{msg}\nContent= {text}";
                }
                _logger.LogError(msg);
            }
            return new ResJson();
        }



    }//class
}
