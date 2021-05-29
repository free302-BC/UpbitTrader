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
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Universe.Coin.Upbit.App
{
    using FindRes = ValueTuple<decimal, decimal, decimal>;//(k, rate, mdd)
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

            _jsonOpt = JsonSerializer.Deserialize<JsonSerializerOptions>(File.ReadAllText(_jsonOptionFile))!;
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
                _set.ApplyMovingAvg = false;
                run_Units(uc);
                _set.ApplyMovingAvg = true;
                run_Units(uc);
                //runMulti(uc);

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
                iw.AddCmd(ConsoleKey.F4, resetHours);                
                iw.AddCmd(ConsoleKey.F5, resetMa);
                iw.AddCmd(ConsoleKey.Spacebar, (m) => ev.Set());
                iw.AddCmd(ConsoleKey.Enter, (m) => ev.Set());

                void resetHours(ConsoleModifiers modifier)
                {
                    if (modifier.HasFlag(ConsoleModifiers.Control))
                    {
                        if (modifier.HasFlag(ConsoleModifiers.Shift))
                            _set.Hours /= 2;
                        else
                            _set.Hours *= 2;
                    }
                    else
                    {
                        _set.Hours += modifier.HasFlag(ConsoleModifiers.Shift) ? -3 : +3;
                    }
                    info($"Reloaded: hours={_set.Hours}, ma={_set.MovingAvgSize}");
                    ev.Set();
                }
                void resetMa(ConsoleModifiers modifier)
                {
                    if (modifier.HasFlag(ConsoleModifiers.Control))
                    {
                        if (modifier.HasFlag(ConsoleModifiers.Shift))
                            _set.MovingAvgSize /= 2;
                        else
                            _set.MovingAvgSize *= 2;
                    }
                    else
                    {
                        _set.MovingAvgSize += modifier.HasFlag(ConsoleModifiers.Shift) ? -3 : +3;
                    }
                    if (_set.MovingAvgSize < 1) _set.MovingAvgSize = 1;
                    info($"Reloaded: hours={_set.Hours}, ma={_set.MovingAvgSize}");
                    ev.Set();
                }
            }
        }

        void run_K(Client uc, decimal k)
        {
            var units = new[]
            { CandleUnit.U240, CandleUnit.U60, CandleUnit.U30, CandleUnit.U15, CandleUnit.U10, CandleUnit.U3, CandleUnit.U1 };
            var results = new FindList2();
            var sb = new StringBuilder();
            var hours = _set.Hours;

            info($"Entering {nameof(run_Units)}()...");
            sb.Clear();
            sb.AppendLine($"-----------[ {(int)(hours / 24)}d {hours % 24}h : ma={(_set.ApplyMovingAvg ? _set.MovingAvgSize : 1)} ]------------");

            foreach (var unit in units)
            {
                var count = (int)(hours * 60 / (int)unit);
                var models = load(unit);
                if (models.Length == 0)
                {
                    models = uc.ApiCandle<CandleMinute>(count: count, unit: unit).ToModels();
                    save(models, unit);
                }

                var x = cast(findK(models, 0, count));
                results.Add((unit, x));
                sb.AppendLine($"{count,6} {unit,6}: {x.k,6:F2} {x.rate,8:F4} {x.mdd,6:F2}%");
            }
            var maxRate = results.Max(x => x.res.rate);
            var max = results.First(x => x.res.rate == maxRate);

            sb.AppendLine("-------------------------------------------");
            sb.AppendLine($"{max.unit,13}: {max.res.k,6:F2} {(max.res.rate - 1) * 100,6:F2}%, {max.res.mdd,6:F2}%");
            sb.AppendLine("-------------------------------------------");
            info(sb.ToString());

            //
            var countMax = (int)(hours * 60 / (int)max.unit);
            runBackTest(uc, countMax, max.res.k, max.unit);
        }

        /// <summary>
        /// 각 CandleUnit 별로 최적 k를 구해 backtest 수행
        /// </summary>
        /// <param name="uc"></param>
        void run_Units(Client uc)
        {
            var units = new[]
            { CandleUnit.U240, CandleUnit.U60, CandleUnit.U30, CandleUnit.U15, CandleUnit.U10, CandleUnit.U3, CandleUnit.U1 };
            var results = new FindList2();
            var sb = new StringBuilder();
            var hours = _set.Hours;

            info($"Entering {nameof(run_Units)}()...");
            sb.Clear();
            sb.AppendLine($"-----------[ {(int)(hours / 24)}d {hours % 24}h : ma={(_set.ApplyMovingAvg ? _set.MovingAvgSize : 1)} ]------------");
            
            foreach (var unit in units)
            {
                var count = (int)(hours * 60 / (int)unit);
                var models = load(unit);
                if (models.Length == 0)
                {
                    models = uc.ApiCandle<CandleMinute>(count: count, unit: unit).ToModels();
                    save(models, unit);
                }

                var x = cast(findK(models, 0, count));
                results.Add((unit, x));
                sb.AppendLine($"{count,6} {unit,6}: {x.k,6:F2} {x.rate,8:F4} {x.mdd,6:F2}%");
            }
            var maxRate = results.Max(x => x.res.rate);
            var max = results.First(x => x.res.rate == maxRate);

            sb.AppendLine("-------------------------------------------");
            sb.AppendLine($"{max.unit,13}: {max.res.k,6:F2} {(max.res.rate - 1) * 100,6:F2}%, {max.res.mdd,6:F2}%");
            sb.AppendLine("-------------------------------------------");
            info(sb.ToString());

            //
            var countMax = (int)(hours * 60 / (int)max.unit);
            runBackTest(uc, countMax, max.res.k, max.unit);
        }


        /// <summary>
        /// 주어진 시간을 3시간 구간으로 나누어 각 구간별 최적 k를 찾아서 backtest 수행
        /// </summary>
        /// <param name="uc"></param>
        void run_Units_3Hours(Client uc)
        {
            var units = new[]
            { /*CandleUnit.U240, */CandleUnit.U60, CandleUnit.U30, CandleUnit.U15, CandleUnit.U10, CandleUnit.U3, CandleUnit.U1 };

            var totalHours = _set.Hours;
            var hours = 3m;
            var numTest = (int)(totalHours / hours);

            info($"Entering {nameof(run_Units_3Hours)}()...");
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

            var models = load(unit);
            if (models.Length == 0)
            {
                models = uc.ApiCandle<CandleMinute>(count: count, unit: unit).ToModels();
                save(models, unit);
            }

            var max = cast(findK(models, 0, count, sb));
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
                    sb?.AppendLine($"{k,6:F2}: {(rate - 1) * 100,10:F2}%, {mdd,10:F2}%");
                }
                var maxRate = list.Max(x => x.rate);
                var max = list.First(x => x.rate == maxRate);
                return max;
            }
        }

        static (decimal k, decimal rate, decimal mdd) cast(FindRes res) => res;

        #endregion


        #region ---- BackTest ----

        void runBackTest(Client uc, int count, decimal k, CandleUnit unit = CandleUnit.U15)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"-------- [ Backtest: {unit}, k={k:F1}, ma={(_set.ApplyMovingAvg ? _set.MovingAvgSize : 1)} ]--------");

            var models = load(unit);
            if (models.Length == 0)
            {
                models = uc.ApiCandle<CandleMinute>(count: count, unit: unit).ToModels();
                save(models, unit);
            }

            var (finalRate, mdd) = backTest(models, k, 0, count);

            if (_set.PrintCandle)
            {
                var tradeModels = models.Take(count).Where(x => x.Rate != 1m);
                var numTrades = tradeModels.Count();
                sb.Append($"{tradeModels.Print(0, count)}")
                    .AppendLine("------------------------------------------------")
                    .Append($"Trades= {numTrades}/{count},")
                    .AppendLine($" Profit Rate= {(finalRate - 1) * 100:F2}%, MDD= {mdd:F2}%")
                    .AppendLine("------------------------------------------------");
            }
            info(sb);
            //File.WriteAllText($"backtest_{unit}_{k}.txt", res);
        }

        (decimal rate, decimal mdd) backTest(CandleModel[] models, decimal k)
            => backTest(models, k, 0, models.Length);
        (decimal rate, decimal mdd) backTest(CandleModel[] models, decimal k, int offset, int count)
        {
            if (!_set.ApplyMovingAvg) SimpleTL.Default.CalcProfitRate(models, k, offset, count);
            else MovingAvgTL.Default.CalcProfitRate(models, k, offset, count); ;

            var rate = ITradeLogic.CalcCumRate(models, offset, count);
            var mdd = ITradeLogic.CalcDrawDown(models, offset, count);
            return (rate, mdd);
        }

        #endregion


        #region ---- Save/Load ----

        JsonSerializerOptions _jsonOpt;
        const string _jsonOptionFile = "api_json_option.json";
        void save(CandleModel[] models, CandleUnit unit)
        {
            var opt = new JsonSerializerOptions(_jsonOpt);
            opt.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.HangulSyllables);
            var json = JsonSerializer.Serialize(models, opt);
            File.WriteAllText($"{unit}.json", json);
        }
        CandleModel[] load(CandleUnit unit)
        {
            var fn = $"{unit}.json";
            if (File.Exists(fn))
            {
                var json = File.ReadAllText(fn);
                var models = JsonSerializer.Deserialize<CandleModel[]>(json, _jsonOpt);
                if (models != null) return models;
            }
            return Array.Empty<CandleModel>();
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
