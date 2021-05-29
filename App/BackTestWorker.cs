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
using System.Diagnostics;
using System.Numerics;

namespace Universe.Coin.Upbit.App
{
    //(decimal k, decimal rate, decimal mdd)
    using FindRes = ValueTuple<decimal, decimal, decimal>;
    using FindList = List<(decimal k, decimal rate, decimal mdd)>;
    using FindList2 = List<(CandleUnit unit, (decimal k, decimal rate, decimal mdd) res)>;

    public class BackTestWorker : WorkerBase<BackTestWorker, BackTestOptions>, IDisposable
    {
        public BackTestWorker(
            ILogger<BackTestWorker> logger,
            IOptionsMonitor<BackTestOptions> set,
            IServiceProvider sp)
            : base(logger, sp, set, GetNewId())
        {
            _ev = new(false);
            onOptionsUpdate += () => _ev.Set();
        }
        public void Dispose() => _ev?.Dispose();

        volatile bool _repeat = false;
        readonly ManualResetEvent _ev;
        protected override void work()
        {
            var logger = _sp.GetRequiredService<ILogger<Client>>();
            var uc = new Client(_set.AccessKey, _set.SecretKey, logger);
            registerHotkey(_ev);

            while (_set.Hours > 0)
            {
                runSingle(uc);
                runMulti(uc);

                if (_repeat)
                {
                    info($"<{Id}> Sleeping 3000...");
                    Thread.Sleep(3000);
                }
                else
                {
                    info($"<{Id}> Waiting...");
                    _ev.Reset();
                    _ev.WaitOne();
                }
            }

            void registerHotkey(EventWaitHandle ev)
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
                iw.AddCmd(ConsoleKey.Spacebar, () => ev.Set());
                iw.AddCmd(ConsoleKey.Enter, () => ev.Set());

                void runHotkey(decimal hours)
                {
                    info($"Reloaded: Hours = {_set.Hours += hours}");
                    ev.Set();
                }
            }
        }

        void runSingle(Client uc)
        {
            var units = new[]
            { CandleUnit.U240, CandleUnit.U60, CandleUnit.U30, CandleUnit.U15, CandleUnit.U10, CandleUnit.U3, CandleUnit.U1 };
            var results = new FindList2();
            var sb = new StringBuilder();
            var hours = _set.Hours;

            info($"Entering {nameof(runSingle)}()...");
            sb.Clear();
            sb.AppendLine($"-----------[ {(int)(hours / 24)}d {hours % 24}h ]------------");
            foreach (var unit in units)
            {
                var count = (int)(hours * 60 / (int)unit);
                var models = uc.ApiCandle<CandleMinute>(count: count, unit: unit).ToModels();
                var x = cast(findK(models));
                results.Add((unit, x));
                sb.AppendLine($"{count,6} {unit,6}: {x.k,6:N2} {x.rate,6:N2}%, {x.mdd,6:N2}%");
            }
            var maxRate = results.Max(x => x.res.rate);
            var max = results.First(x => x.res.rate == maxRate);

            sb.AppendLine("-------------------------------------------");
            sb.AppendLine($"{max.unit,6}: {max.res.k,6:N2}: {max.res.rate,6:N2}%, {max.res.mdd,6:N2}%");
            info(sb.ToString());
        }

        void runMulti(Client uc)
        {
            var units = new[]
            { /*CandleUnit.U240, */CandleUnit.U60, CandleUnit.U30, CandleUnit.U15, CandleUnit.U10, CandleUnit.U3, CandleUnit.U1 };

            var totalHours = _set.Hours;
            var hours = 3m;
            var numTest = (int)(totalHours / hours);

            info($"Entering {nameof(runMulti)}()...");
            var sb = new StringBuilder();
            sb.AppendLine($"-----------[ {(int)(hours / 24)}d {hours % 24}h ]------------");

            var kDic = new Dictionary<CandleUnit, FindList>();//
            var summary = new List<(CandleUnit unit, BigInteger rate)>();
            foreach (var unit in units)
            {
                var count = (int)(hours * 60 / (int)unit);
                var totalCount = (int)(totalHours * 60 / (int)unit);
                var models = uc.ApiCandle<CandleMinute>(count: totalCount, unit: unit).ToModels();

                var results = new FindList();
                sb.Append($"{unit,6}: ");

                for (int i = 0; i < numTest; i++)
                {
                    //var currents = new ArraySegment<CandleModel>(models, count * i, count);
                    var x = cast(findK(models, count * i, count));
                    results.Add(x);
                    //sb.Append($"{(x.rate - 1) * 100,6:N2} ");
                }
                kDic.Add(unit, results);

                //sb.AppendLine("-------------------------------------------");
                var seed = BigInteger.One;
                var cumRate0 = results.Aggregate(1m, (p, x) => p * x.rate);
                var cumRateInteger = results.Aggregate(seed, (p, x) => p * (int)(10000 * x.rate));
                var cumRate = cumRateInteger / BigInteger.Pow(10000, results.Count - 1) - 10000;

                sb.AppendLine($"| {cumRate,6}%%");
                //sb.AppendLine("-------------------------------------------");
                summary.Add((unit, cumRate));
            }
            var maxRate = summary.Max(x => x.rate);
            var max = summary.First(x => x.rate == maxRate);
            sb.AppendLine("-------------------------------------------");
            sb.AppendLine($"{max.unit,6}: {max.rate}%%");
            info(sb);

            sb.Clear();
            foreach (var u in kDic.Keys) sb.Append($"{u,12} ");
            sb.AppendLine();
            for (int i = 0; i < numTest; i++)
            {
                foreach (var key in kDic.Keys) sb.Append($"{kDic[key][i].k,4:F1} {kDic[key][i].rate,6}% ");
                sb.AppendLine();
            }
            info(sb);
        }


        #region ---- Find K ----

        FindRes runFindK(Client uc, int count, CandleUnit unit = CandleUnit.U60)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"--- Finding K: {count} {unit} ----");

            var models = uc.ApiCandle<CandleMinute>(count: count, unit: unit).ToModels();
            var max = cast(findK(models, sb));
            max.rate = Math.Round((max.rate - 1) * 100, 2);
            sb.AppendLine("---------------------------------------------------");
            sb.AppendLine($"{max.k,6:N2}: {max.rate,10:N2}%, {max.mdd,10:N2}%");
            info(sb);
            return max;
        }

        FindRes findK(CandleModel[] models, StringBuilder? sb = null)
            => findK(models, 0, models.Length, sb);

        FindRes findK(CandleModel[] models, int offset, int count, StringBuilder? sb = null)
        {
            if (models.Length == 0) return (0m, 0m, 0m);
            else
            {
                var list = new FindList();// (CandleUnit unit, decimal k, decimal rate, decimal mdd)>();
                for (decimal k = 0.1m; k <= 2m; k += 0.1m)
                {
                    var (rate, mdd) = backTest(models, k, offset, count);
                    list.Add((k, rate, mdd));
                    sb?.AppendLine($"{k,6:N2}: {(rate - 1) * 100,10:N2}%, {mdd,10:N2}%");
                }
                var maxRate = list.Max(x => x.rate);
                var max = list.First(x => x.rate == maxRate);
                return max;
            }
        }

        static (decimal k, decimal rate, decimal mdd) cast(FindRes res) => res;

        #endregion


        #region ---- BackTest ----

        void backTest(Client uc, int count, decimal k, CandleUnit unit = CandleUnit.U15)
        {
            var data = uc.ApiCandle<CandleMinute>(count: count, unit: unit);
            //info(IApiModel.Print(data));

            var models = data.ToModels();
            var (finalRate, mdd) = backTest(models, k);

            var lossModels = IViewModel.Print(models.Where(x => x.Rate < 1));
            var res = IViewModel.Print(models);
            //File.WriteAllText($"backtest_{unit}_{k}.txt", res);
            if (_set.PrintCandle) info(lossModels);
            //info($"Final Profit Rate= {(finalRate - 1) * 100:N2}%", $"MDD= {mdd:N2}%");
        }

        (decimal rate, decimal mdd) backTest(CandleModel[] models, decimal k)
            => backTest(models, k, 0, models.Length);
        (decimal rate, decimal mdd) backTest(CandleModel[] models, decimal k, int offset, int count)
        {
            BuyOverDelta.Default.CalcProfitRate(models, k, offset, count);

            var rate = ITradeLogic.CalcCumRate(models, offset, count);
            var mdd = ITradeLogic.CalcDrawDown(models, offset, count);
            return (rate, mdd);
        }

        #endregion


        #region ---- Worker ID ----

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
        void saveKey(WorkerOptions set)
        {
            Helper.SaveEncrptedKey(set.AccessKey, set.SecretKey, "key.txt");
        }


        #endregion

    }//class
}
