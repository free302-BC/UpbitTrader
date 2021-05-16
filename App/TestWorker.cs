using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Universe.AppBase;
using System.Text.Json;

namespace Universe.Coin.Upbit.App
{
    using ResJson = List<Dictionary<string, object>>;
    public class TestWorker : WorkerBase<TestWorker, WorkerSetting>
    {
        public TestWorker(ILogger<TestWorker> logger, IOptionsMonitor<WorkerSetting> set) : base(logger, set) { }

        protected override void work(WorkerSetting set)
        {
            try
            {
                candle(set);
            }
            catch (Exception e)
            {
                log("work():\n" + e.Message);
            }
        }

        void candle(WorkerSetting set)
        {
            var api = Api.CandleDays;
            var nvc = HttpUtility.ParseQueryString(string.Empty);
            nvc.Add("market", "KRW-BTC");// string | 마켓 코드 (ex. KRW-BTC) 
            nvc.Add("to", "");// string | 마지막 캔들 시각 (exclusive). 포맷 : `yyyy-MM-dd'T'HH:mm:ssXXX` or `yyyy-MM-dd HH:mm:ss`. 비워서 요청 시 가장 최근 캔들  (optional) 
            nvc.Add("count", "8");  // decimal? | 캔들 개수  (optional) 
            nvc.Add("convertingPriceUnit", "KRW");

            var wc = new WebClient();
            wc.QueryString.Add(nvc);
            wc.BuildAuthToken(set);//, nvc);
            wc.Headers["Accept"] = "application/json";

            try
            {
                var url = Helper.GetApiUrl(api);
                info(url);
                var response = wc.DownloadString(url);
                var list = JsonSerializer.Deserialize<ResJson>(response) ?? new ResJson();

                var sb = new StringBuilder();
                foreach (var dic in list)
                {
                    sb.Clear();
                    foreach (var k in dic.Keys) sb.AppendLine($"{k,-30}=\t{dic[k]}");
                    info(sb);
                }
            }
            catch (Exception ex)
            {
                log($"Execption from '{api}':\n{ex.Message}");
            }
        }

    }//class
}
