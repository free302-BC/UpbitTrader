using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
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
    using FindRes = ValueTuple<CandleUnit, decimal, decimal, decimal>;

    public class BackTestWorker : WorkerBase<BackTestWorker, WorkerSetting>
    {
        public BackTestWorker(
            ILogger<BackTestWorker> logger,
            IOptionsMonitor<WorkerSetting> set,
            IServiceProvider sp,
            IConfigurationRoot config)
            : base(logger, set, sp)
        {
            _testSettings = config.GetSection("BackTestSettings");
        }
        readonly IConfigurationSection _testSettings;
        decimal _hours => _testSettings.GetValue<decimal>("Hours");
        bool _printCandle => _testSettings.GetValue<int>("PrintCandle") != 0;
        bool _applyStopLoss => _testSettings.GetValue<int>("ApplyStopLoss") != 0;

        protected override void work()
        {
            var logger = _sp.GetRequiredService<ILogger<Client>>();
            var uc = new Client(_set.AccessKey, _set.SecretKey, logger);
            try
            {
                run(uc);
            }
            catch (Exception e)
            {
                log("work():", e.Message);
            }
        }

        void run(Client uc)
        {
            var units = new[] { CandleUnit.U240, CandleUnit.U60, CandleUnit.U30, CandleUnit.U15, CandleUnit.U10, CandleUnit.U3, CandleUnit.U1 };
            var results = new List<(CandleUnit unit, decimal k, decimal rate, decimal mdd)>();
            var sb = new StringBuilder();

            var hours = _hours;
            while (true)
            {
                results.Clear();
                sb.Clear();

                //var hours = _hours;
                sb.AppendLine($"-----------[ {(int)(hours / 24)}.{hours % 24}, SL:{_applyStopLoss} ]------------");

                //for (int i = 0; i < 20; i++)
                foreach (var unit in units)
                {
                    //Thread.Sleep(100);
                    //hours += 1 * 20;
                    //var unit = CandleUnit.U60;
                    var count = (int)(hours * 60 / (int)unit);
                    var x = to(findK(uc, count, unit));
                    results.Add(x);
                    sb.AppendLine($"{count,6} {x.unit,6}: {x.k,6:N2} {x.rate,6:N2}%, {x.mdd,6:N2}%");
                }
                //info(sb.ToString());

                var maxRate = results.Max(x => x.rate);
                var max = results.First(x => x.rate == maxRate);

                sb.AppendLine("-------------------------------------------");
                sb.AppendLine($"{max.unit,6}: {max.k,6:N2}: {max.rate,6:N2}%, {max.mdd,6:N2}%");
                info(sb.ToString());

                //backTest(uc, (int)(hours * 60 / (int)max.unit), max.k, max.unit);
                Thread.Sleep(1000);
                hours += 3;
            }

            (CandleUnit unit, decimal k, decimal rate, decimal mdd) to(FindRes res) => res;
        }

        FindRes findK(Client uc, int count, CandleUnit unit = CandleUnit.U60)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"--- Finding K: count= {count} ----");

            var models = uc.ApiCandle<CandleMinute>(count: count, unit: unit).ToModels();
            (CandleUnit unit, decimal k, decimal rate, decimal mdd) max;
            if (models.Count == 0) max = (unit, 0m, 0m, 0m);
            else
            {
                var list = new List<(CandleUnit unit, decimal k, decimal rate, decimal mdd)>();
                for (decimal k = 0.1m; k <= 1.5m; k += 0.1m)
                {
                    //CandleModel.CalcRate(models, k);
                    var (rate, mdd) = calcBackTest(models, k);
                    list.Add((unit, k, rate, mdd));
                    sb.AppendLine($"{k,6:N2}: {(rate - 1) * 100,10:N2}%, {mdd,10:N2}%");
                }
                var maxRate = list.Max(x => x.rate);
                max = list.First(x => x.rate == maxRate);
                max.rate = Math.Round((max.rate - 1) * 100, 2);

                sb.AppendLine("---------------------------------------------------");
                sb.AppendLine($"{max.k,6:N2}: {max.rate,10:N2}%, {max.mdd,10:N2}%");                
            }
            //info(sb);
            return max;
        }

        void backTest(Client uc, int count, decimal k, CandleUnit unit = CandleUnit.U15)
        {
            var data = uc.ApiCandle<CandleMinute>(count: count, unit: unit);
            //info(IApiModel.Print(data));

            var models = data.ToModels();
            var (finalRate, mdd) = calcBackTest(models, k);

            var down = IViewModel.Print(models.Where(x => x.Rate < 1));
            var res = IViewModel.Print(models);
            //File.WriteAllText($"backtest_{unit}_{k}.txt", res);
            if (_printCandle) info(down);
            //info($"Final Profit Rate= {(finalRate - 1) * 100:N2}%", $"MDD= {mdd:N2}%");
        }

        (decimal rate, decimal mdd) calcBackTest(List<CandleModel> models, decimal k)
        {
            CandleModel.CalcRate(models, k, _applyStopLoss);
            var rate = CandleModel.CalcCumRate(models);
            var mdd = CandleModel.CalcDrawDown(models);
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
        void saveKey(WorkerSetting set)
        {
            Helper.SaveEncrptedKey(set.AccessKey, set.SecretKey, "key.txt");
        }

        #endregion

    }//class
}
