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
                backTest(uc, 15, 0.4);
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

        void backTest(UpbitClient uc, int count, double k)
        {
            var data = uc.ApiCandleDay(count);
            var first = data.First();
            //printJsonRes(data);
            info(first);

            var target = (first as ICalcModel).NextTarget(k);
            info(target);

            var sb = new StringBuilder();
            var models = data.Select(x => new CalcModel(x, k)).ToList();
            var totalRate = 1.0;
            for (int i = 1; i < models.Count; i++)
            {
                totalRate *= models[i].CalcRate(models[i - 1].NextTarget);
                sb.AppendLine(models[i].ToString());
            }
            info(sb);
            totalRate = Math.Round((totalRate - 1) * 100, 2);
            info($"rate= {totalRate}%");
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
