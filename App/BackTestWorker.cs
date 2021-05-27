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
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.Upbit.App
{
    using FindRes = ValueTuple<CandleUnit, decimal, decimal, decimal>;
    //(CandleUnit unit, decimal k, decimal rate, decimal mdd)

    public class BackTestWorker : WorkerBase<BackTestWorker, BackTestOptions>
    {
        public BackTestWorker(
            ILogger<BackTestWorker> logger,
            IOptionsMonitor<BackTestOptions> set,
            IServiceProvider sp)
            : base(logger, sp, set, GetNewId())
        {
        }

        volatile bool _repeat = false;
        protected override void work()
        {
            var logger = _sp.GetRequiredService<ILogger<Client>>();
            var uc = new Client(_set.AccessKey, _set.SecretKey, logger);
            using var ev = new AutoResetEvent(false);
            registerHotkey(ev);
            while (_set.Hours > 0)
            {
                runSingle(uc);
                runMulti(uc);
                //Thread.Sleep(3000);
                info($"<{Id}> Waiting...");

                if (_repeat) Thread.Sleep(3000);
                else ev.WaitOne();
            }
        }

        private void registerHotkey(AutoResetEvent ev)
        {
            var iw = _sp.GetRequiredService<InputWorker>();
            iw.AddCmd(ConsoleKey.F4, () => runHotkey(_set.Hours));
            iw.AddCmd(ConsoleKey.F3, () => runHotkey(3m));
            iw.AddCmd(ConsoleKey.F2, () => runHotkey(-3m));
            iw.AddCmd(ConsoleKey.F1, () => runHotkey(-_set.Hours / 2));
            iw.AddCmd(ConsoleKey.F5, () =>
            {
                _repeat = !_repeat;
                info($"Reloaded: Repeat = {_repeat}");
                if (_repeat) ev.Set();
            });
            
            void runHotkey(decimal hours)
            {
                info($"Reloaded: Hours = {_set.Hours += hours}");
                ev.Set();
            }
        }

        static (CandleUnit unit, decimal k, decimal rate, decimal mdd) cast(FindRes res) => res;
        static List<(CandleUnit unit, decimal k, decimal rate, decimal mdd)> cast(IList<FindRes> res)
            => (List<(CandleUnit unit, decimal k, decimal rate, decimal mdd)>)res;

        void runSingle(Client uc)
        {
            var units = new[]
            { CandleUnit.U240, CandleUnit.U60, CandleUnit.U30, CandleUnit.U15, CandleUnit.U10, CandleUnit.U3, CandleUnit.U1 };
            var results = new List<FindRes>();
            var sb = new StringBuilder();
            var hours = _set.Hours;

            sb.Clear();
            sb.AppendLine($"-----------[ {(int)(hours / 24)}d {hours % 24}h ]------------");
            foreach (var unit in units)
            {
                var count = (int)(hours * 60 / (int)unit);
                var x = cast(findK(uc, count, unit));
                results.Add(x);
                sb.AppendLine($"{count,6} {x.unit,6}: {x.k,6:N2} {x.rate,6:N2}%, {x.mdd,6:N2}%");
            }
            var maxRate = cast(results).Max(x => x.rate);
            var max = cast(results).First(x => x.rate == maxRate);

            sb.AppendLine("-------------------------------------------");
            sb.AppendLine($"{max.unit,6}: {max.k,6:N2}: {max.rate,6:N2}%, {max.mdd,6:N2}%");
            info(sb.ToString());
        }

        void runMulti(Client uc)
        {
            var units = new[]
            { /*CandleUnit.U240, */CandleUnit.U60, CandleUnit.U30, CandleUnit.U15, CandleUnit.U10, CandleUnit.U3, CandleUnit.U1 };

            var totalHours = _set.Hours;
            var hours = 3m;
            var numTest = (int)(totalHours / hours);

            var sb = new StringBuilder();
            sb.AppendLine($"-----------[ {(int)(hours / 24)}d {hours % 24}h ]------------");

            var kDic = new Dictionary<CandleUnit, List<FindRes>>();//
            var summary = new List<(CandleUnit unit, decimal rate)>();
            foreach (var unit in units)
            {
                var count = (int)(hours * 60 / (int)unit);
                var totalCount = (int)(totalHours * 60 / (int)unit);
                var models = uc.ApiCandle<CandleMinute>(count: totalCount, unit: unit).ToModelArray();

                var results = new List<FindRes>();
                sb.Append($"{unit,6}: ");
                for (int i = 0; i < numTest; i++)
                {
                    var currents = new ArraySegment<CandleModel>(models, count * i, count);
                    var x = cast(findK(currents, unit));
                    results.Add(x);
                    //sb.Append($"{(x.rate - 1) * 100,6:N2} ");
                }
                //sb.AppendLine("-------------------------------------------");
                var cumRate = 100 * (cast(results).Aggregate(1m, (p, x) => p * x.rate) - 1);
                sb.AppendLine($"| {cumRate,6:N2}%");
                //sb.AppendLine("-------------------------------------------");
                summary.Add((unit, cumRate));
                kDic.Add(unit, results);
            }
            var maxRate = summary.Max(x => x.rate);
            var max = summary.First(x => x.rate == maxRate);
            sb.AppendLine("-------------------------------------------");
            sb.AppendLine($"{max.unit,6}: {max.rate,6:N2}%");
            info(sb);

            sb.Clear();
            foreach (var x in kDic[max.unit]) sb.AppendLine(x.ToString());
            info(sb);
        }

        FindRes findK(Client uc, int count, CandleUnit unit = CandleUnit.U60)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"--- Finding K: count= {count} ----");

            var models = uc.ApiCandle<CandleMinute>(count: count, unit: unit).ToModels();
            var max = cast(findK(models, unit, sb));
            max.rate = Math.Round((max.rate - 1) * 100, 2);
            sb.AppendLine("---------------------------------------------------");
            sb.AppendLine($"{max.k,6:N2}: {max.rate,10:N2}%, {max.mdd,10:N2}%");
            //info(sb);
            return max;
        }

        FindRes findK(IList<CandleModel> models, CandleUnit unit, StringBuilder? sb = null)
        {
            if (models.Count == 0) return (unit, 0m, 0m, 0m);
            else
            {
                var list = new List<FindRes>();// (CandleUnit unit, decimal k, decimal rate, decimal mdd)>();
                for (decimal k = 0.1m; k <= 2m; k += 0.1m)
                {
                    //CandleModel.CalcRate(models, k);
                    var (rate, mdd) = backTest(models, k);
                    list.Add((unit, k, rate, mdd));
                    sb?.AppendLine($"{k,6:N2}: {(rate - 1) * 100,10:N2}%, {mdd,10:N2}%");
                }
                var maxRate = cast(list).Max(x => x.rate);
                var max = cast(list).First(x => x.rate == maxRate);
                return max;
            }
        }

        void backTest(Client uc, int count, decimal k, CandleUnit unit = CandleUnit.U15)
        {
            var data = uc.ApiCandle<CandleMinute>(count: count, unit: unit);
            //info(IApiModel.Print(data));

            var models = data.ToModels();
            var (finalRate, mdd) = backTest(models, k);

            var down = IViewModel.Print(models.Where(x => x.Rate < 1));
            var res = IViewModel.Print(models);
            //File.WriteAllText($"backtest_{unit}_{k}.txt", res);
            if (_set.PrintCandle) info(down);
            //info($"Final Profit Rate= {(finalRate - 1) * 100:N2}%", $"MDD= {mdd:N2}%");
        }

        (decimal rate, decimal mdd) backTest(IList<CandleModel> models, decimal k)
        {
            CandleModel.CalcRate(models, k, _set.ApplyStopLoss);
            var rate = CandleModel.CalcCumRate(models);
            var mdd = CandleModel.CalcDrawDown(models);
            return (rate, mdd);
        }

        #region ---- Work ID ----

        static object _lock;
        static List<string> _ids;
        static volatile int _index;
        static BackTestWorker()
        {
            _ids = new();
            _index = 0;
            _lock = new();

            //TODO: load from json or DB
            for (int i = 0; i < 3; i++) _ids.Add($"{nameof(BackTestWorker)}:{i + 1}");
        }
        public static string GetNewId()
        {
            lock (_lock) return _ids[_index++];
        }
        public static List<string> GetIds() => _ids;

        #endregion


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
        void saveKey(WorkerOptionsBase set)
        {
            Helper.SaveEncrptedKey(set.AccessKey, set.SecretKey, "key.txt");
        }

        #endregion

    }//class
}
