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
using System.IO;
using System.Collections.Specialized;

namespace Universe.Coin.Upbit.App
{
    public class TestWorker : WorkerBase<TestWorker, WorkerSetting>
    {
        public TestWorker(ILogger<TestWorker> logger, IOptionsMonitor<WorkerSetting> set, IServiceProvider sp)
            : base(logger, set, sp) { }

        protected override void work(WorkerSetting set)
        {
            var logger = _sp.GetRequiredService<ILogger<Client>>();
            var uc = new Client(set.CheckAuthKey(), logger);
            try
            {
                findK(uc, 30);
                //backTest(uc, 7, 0.46);
                //backTest(uc, 30, 0.46);
                //backTest(uc, 90, 0.46);
            }
            catch (Exception e)
            {
                log("work():", e.Message);
            }
        }

        void findK(Client uc, int count)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"--- Finding K: count= {count} ----");
            var list = new List<(double k, double rate, double mdd)>();
            var models = uc.ApiDayModels(count);
            for (double k = 0.1; k <= 1.0; k += 0.1)
            {
                CalcModel.CalcRate(models, k);
                var (rate, mdd) = backTest(models, k);
                list.Add((k, rate, mdd));
                sb.AppendLine($"{k,6:N2}: {(rate - 1) * 100,10:N2}%, {mdd,10:N2}%");
            }
            var maxRate = list.Max(x => x.rate);
            var max = list.First(x => x.rate == maxRate);
            sb.AppendLine("---------------------------------------------------");
            sb.AppendLine($"{max.k,6:N2}: {(max.rate - 1) * 100,10:N2}%, {max.mdd,10:N2}%");
            info(sb);
        }
        
        
        void backTest(Client uc, int count, double k)
        {
            var data = uc.ApiCandle<CandleDay>(count);
            //info(ICandle.Print(data));

            var models = data.Select(x => x.ToModel()).Reverse().ToList();
            var (finalRate, mdd) = backTest(models, k);

            info(CalcModel.Print(models));
            info($"Final Profit Rate= {(finalRate - 1) * 100:N2}%", $"MDD= {mdd:N2}%");
        }

        private static (double rate, double mdd) backTest(List<CalcModel> models, double k)
        {
            CalcModel.CalcRate(models, k);
            var rate = CalcModel.CalcCumRate(models);
            var mdd = CalcModel.CalcDrawDown(models);
            return (rate, mdd);
        }

        #region ---- TEST ----

        void testLimit(Client uc)
        {
            var list = uc.ApiCandle<CandleDay>();
            //printResJson(list);
            info("--------------");

            for (int i = 0; i < 10; i++)
            {
                list = uc.ApiCandle<CandleDay>();
                if (list.Count < 8) break;
                info(list[0]);
                Thread.Sleep(10);
            }
        }
        void testHash()
        {
            var nvc = new NameValueCollection();
            nvc.Add("count", "123");
            Helper.buildQueryHash(nvc);
        }

        #endregion

    }//class
}
