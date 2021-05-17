using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Universe.AppBase;
using System.Text.Json;
using Universe.Coin.Upbit.Model;

namespace Universe.Coin.Upbit.App
{
    using JsonRes = List<CandleDay>;

    public class TestWorker : WorkerBase<TestWorker, WorkerSetting>
    {
        public TestWorker(ILogger<TestWorker> logger, IOptionsMonitor<WorkerSetting> set, IServiceProvider sp) 
            : base(logger, set, sp) { }

        protected override void work(WorkerSetting set)
        {
            var logger = _sp.GetRequiredService<ILogger<UpbitClient>>();
            var uc = new UpbitClient(set, logger);
            try
            {
                backTest(uc, 15, 0.4f);
            }
            catch (Exception e)
            {
                log("work():\n" + e.Message);
            }
        }

        void printJsonRes(JsonRes list)
        {
            var sb = new StringBuilder();
            foreach (var dic in list) sb.AppendLine(dic.ToString()); //printDic(dic, sb, inLine);
            info(sb);
        }

        void backTest(UpbitClient uc, int count, float k)
        {
            var data = uc.ApiCandleDay();
            var first = data.First();
            //printJsonRes(data);
            info(first);

            var target = (first as ICalcModel).NextTarget(k);
            info(target);


            var sum = 0.0;
            var fee = 0.0015;
            var models = data.Select(x => x as ICalcModel).ToList();
            for (int i = 1; i < models.Count; i++)
            {
                var m = models[i];
                var m0 = models[i - 1];
                if (m.HighPrice > m0.NextTarget(k)) sum += m.TradePrice / m0.NextTarget(k) - fee;
                else sum += 1.0;

            }

            models.Aggregate(0.0, (s, x) => s += x.NextTarget(k));

        }
        void testLimit(UpbitClient uc)
        {
            var list = uc.ApiCandleDay();
            //printResJson(list);
            info("--------------");

            for (int i = 0; i < 10; i++)
            {
                list = uc.ApiCandleDay();
                if (list.Count < 8) break;
                info(list[0]);
                Thread.Sleep(10);
            }
        }

    }//class
}
