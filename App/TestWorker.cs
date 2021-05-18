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
        
        void backTest(UpbitClient uc, int count, double k)
        {
            var data = uc.ApiCandleDay(count);
            info(ICandle.Print(data));

            var models = data.Select(x => new CalcModel(x)).Reverse().ToList();
            CalcModel.CalcRate(models, k);
            var finalRate = CalcModel.CalcCumRate(models);
            var mdd = CalcModel.CalcDrawDown(models);

            info(CalcModel.Print(models));
            info($"Final Profit Rate= {(finalRate - 1) * 100:N2}%\r\nMDD= {mdd:N2}%");
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
