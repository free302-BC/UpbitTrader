using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Universe.Coin.Upbit.Model;

namespace Universe.Coin.Upbit
{
    using JsonRes = List<CandleDay>;

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

        public JsonRes ApiCandleDay(int count = 15)
        {
            var api = Api.CandleDays;
            var nvc = HttpUtility.ParseQueryString(string.Empty);
            nvc.Add("market", "KRW-BTC");// string | 마켓 코드 (ex. KRW-BTC) 
            nvc.Add("to", "");// string | 마지막 캔들 시각 (exclusive). 포맷 : `yyyy-MM-dd'T'HH:mm:ssXXX` or `yyyy-MM-dd HH:mm:ss`. 비워서 요청 시 가장 최근 캔들  (optional) 
            nvc.Add("count", count.ToString());
            nvc.Add("convertingPriceUnit", "KRW");
            _wc.QueryString.Clear();
            _wc.QueryString.Add(nvc);

            try
            {
                var response = _wc.DownloadString(Helper.GetApiUrl(api));
                var list = JsonConvert.DeserializeObject<JsonRes>(response) ?? new JsonRes();
                return list;
            }
            catch (WebException ex)
            {
                var msg = $"API={api}: uri = { ex.Response?.ResponseUri}\nstatus={ex.Status}: {ex.Message}";
                if (ex.Response != null)
                {
                    using var sr = new StreamReader(ex.Response.GetResponseStream());
                    var text = sr.ReadToEnd();
                    msg = $"{msg}\nContent= {text}";
                }
                _logger.LogError(msg);
            }
            return new JsonRes();
        }



    }//class
}
