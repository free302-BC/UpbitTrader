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

namespace Universe.Coin.Upbit.App
{
    using ResJson = List<Dictionary<string, object>>;
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
                var list = uc.ApiCandleDay();
                //printResJson(list);
                info("--------------");

                for (int i = 0; i < 10; i++)
                {
                    list = uc.ApiCandleDay();
                    if (list.Count < 8) break;
                    printDic(list[0]);
                    //Thread.Sleep(10);
                }
            }
            catch (Exception e)
            {
                log("work():\n" + e.Message);
            }
        }

        void printResJson(ResJson list)
        {
            var sb = new StringBuilder();
            foreach (var dic in list) printDIc(dic, sb);
        }

        void printDIc(Dictionary<string, object> dic, StringBuilder sb)
        {
            sb.Clear();
            foreach (var k in dic.Keys) sb.AppendLine($"{k,-30}=\t{dic[k]}");
            info(sb);
        }
        void printDic(Dictionary<string, object> dic)
        {
            var sb = new StringBuilder();
            foreach (var k in dic.Keys) sb.AppendLine($"{k,-30}=\t{dic[k]}");
            info(sb);
        }


    }//class
}
