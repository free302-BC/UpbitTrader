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

        public UpbitClient(string token, ILogger logger)
        {
            _logger = logger;
            _wc = new WebClient();
            _wc.SetAuthToken(token);
            _wc.SetAcceptance();
            _wc.SetQueryString(("market", "KRW-BTC"), ("count", "1"));
            //nvc.Add("to", "");// string | 마지막 캔들 시각 (exclusive). 포맷 : `yyyy-MM-dd'T'HH:mm:ssXXX` or `yyyy-MM-dd HH:mm:ss`. 비워서 요청 시 가장 최근 캔들  (optional) 
            //nvc.Add("convertingPriceUnit", "KRW");
        }
        public void Dispose()
        {
            _wc?.Dispose();
        }

        public JsonRes ApiCandleDay(int count = 15)
        {
            _wc.SetQueryString(("count", count.ToString()));
            var api = ApiId.CandleDays;
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
