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
using System.Linq.Expressions;

namespace Universe.Coin.Upbit.App
{
    using FindRes = ValueTuple<int, decimal, decimal, decimal>;//(k, rate, mdd)
    using FindList = List<(int trades, decimal k, decimal rate, decimal mdd)>;
    using FindList2 = List<(CandleUnit unit, int count, (int trades, decimal k, decimal rate, decimal mdd) res)>;

    using BtRes = ValueTuple<int, decimal, decimal>;//(trades, rate, mdd)
    using BtList = List<(CandleUnit unit, int count, (int trades, decimal rate, decimal mdd) res)>;

    public class BackTestWorker : WorkerBase<BackTestWorker, BackTestOptions>, IDisposable
    {
        #region ---- Ctor ----

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

        #endregion

        static (int trades, decimal k, decimal rate, decimal mdd) cast(FindRes res) => res;
        static (int trades, decimal rate, decimal mdd) cast(BtRes res) => res;

        volatile bool _repeat = false;
        volatile bool _doFindK = false;
        readonly ManualResetEvent _ev;
        protected override void work()
        {
            var logger = _sp.GetRequiredService<ILogger<Client>>();
            var uc = new Client(_set.AccessKey, _set.SecretKey, logger);
            registerHotkey(_ev);

            while (_set.Hours > 0)
            {
                if(!_doFindK)
                {
                    _set.ApplyMovingAvg = false;
                    run_K_Units(uc);
                    _set.ApplyMovingAvg = true;
                    run_K_Units(uc);
                }
                else
                {
                    _set.ApplyMovingAvg = false;
                    run_Units_FindK(uc);
                    _set.ApplyMovingAvg = true;
                    run_Units_FindK(uc);
                }

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
                iw.AddCmd(ConsoleKey.F1, onFuncChange);
                iw.AddCmd(ConsoleKey.F2, m => onParamChange(m, 3, () => _set.Hours));
                iw.AddCmd(ConsoleKey.F3, m => onParamChange(m, 0.1m, () => _set.FactorK));
                iw.AddCmd(ConsoleKey.F4, m => onParamChange(m, 3, () => _set.MovingAvgSize));
                iw.AddCmd(ConsoleKey.Spacebar, (m) => ev.Set());
                iw.AddCmd(ConsoleKey.Enter, (m) => ev.Set());

                void onFuncChange(ConsoleModifiers modifier)
                {
                    _doFindK = !_doFindK;
                    _ev.Set();
                }
                void onParamChange<P>(ConsoleModifiers modifier, P delta, Expression<Func<P>> selector)
                {
                    var set = selector.ToDelegate();
                    dynamic dv = delta!;
                    dynamic v0 = set.currentValue!;

                    if (modifier.HasFlag(ConsoleModifiers.Control))
                    {
                        if (modifier.HasFlag(ConsoleModifiers.Shift))
                            set.setter(v0 / 2);
                        else
                            set.setter(v0 * 2);
                    }
                    else
                    {
                        set.setter(
                            set.currentValue 
                            + (modifier.HasFlag(ConsoleModifiers.Shift) ? -dv : +dv));
                    }
                    info($"Reloaded: k={_set.FactorK}, hours={_set.Hours}, ma={_set.MovingAvgSize}");
                    ev.Set();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="uc"></param>
        /// <param name="k"></param>
        void run_K_Units(Client uc)
        {
            var units = new[]
            { CandleUnit.M240, CandleUnit.M60, CandleUnit.M30, CandleUnit.M15, CandleUnit.M10, CandleUnit.M3, CandleUnit.M1 };
            var results = new BtList();
            var sb = new StringBuilder();
            var hours = _set.Hours;
            var k = _set.FactorK;

            info($"Entering {nameof(run_K_Units)}()...");
            sb.Clear();
            sb.AppendLine($"-----------[ {(int)(hours / 24)}d {hours % 24}h : k={k:F1} ma={(_set.ApplyMovingAvg ? _set.MovingAvgSize : 1)} ]------------");

            foreach (var unit in units)
            {
                var count = (int)(hours * 60 / (int)unit);
                var models = prepareModels(uc, unit, count);

                var x = IBackTest.backTest(models, k, 0, count, _set.ApplyMovingAvg);
                results.Add((unit, count, x));
                sb.AppendLine($"{count,6} {unit,6}: {k,6:F2} {x.rate,8:F4} {x.mdd,6:F2}%");
            }
            var maxRate = results.Max(x => x.res.rate);
            var max = results.First(x => x.res.rate == maxRate);

            sb.AppendLine("-------------------------------------------");
            sb.Append($"{max.count,6} {max.unit,6}: {k,6:F2} {(max.res.rate - 1) * 100,7:F2}% {max.res.mdd,6:F2}%");
            sb.AppendLine($" | {max.res.trades}/{max.count}Tr");
            sb.AppendLine("-------------------------------------------");

            info(sb.ToString());
        }


        /// <summary>
        /// 각 CandleUnit 별로 최적 k를 구해 backtest 수행
        /// 전체 구간에 대한 최적 k를 구했을 경우 ~ 고정 k에 대한 이론적 최대 수익율
        /// </summary>
        /// <param name="uc"></param>
        void run_Units_FindK(Client uc)
        {
            var units = new[]
            { CandleUnit.M240, CandleUnit.M60, CandleUnit.M30, CandleUnit.M15, CandleUnit.M10, CandleUnit.M3, CandleUnit.M1 };
            var results = new FindList2();
            var sb = new StringBuilder();
            var hours = _set.Hours;

            info($"Entering {nameof(run_Units_FindK)}()...");
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

                var x = IFindK.findK(models, 0, count, _set.ApplyMovingAvg);
                results.Add((unit, count, x));
                sb.AppendLine($"{count,6} {unit,6}: {x.k,6:F2} {x.rate,8:F4} {x.mdd,6:F2}%");
            }
            var maxRate = results.Max(x => x.res.rate);
            var max = results.First(x => x.res.rate == maxRate);

            sb.AppendLine("-------------------------------------------");
            sb.Append($"{max.count,6} {max.unit,6}: {max.res.k,6:F2} {(max.res.rate - 1) * 100,6:F2}%, {max.res.mdd,6:F2}%");
            sb.AppendLine($" | {max.res.trades}/{max.count}Tr");
            sb.AppendLine("-------------------------------------------");
            info(sb.ToString());

            //
            //runBackTest(uc, max.unit, max.count, max.res.k);
        }


        /// <summary>
        /// 주어진 시간을 3시간 구간으로 나누어 각 구간별 최적 k를 찾아서 backtest 수행
        /// 각3시간 구간별 최적 k를 찾았을 경우 ~ 변동 k에 대한 이론적 최대 이익
        /// </summary>
        /// <param name="uc"></param>
        void run_Units_FindK_3Hours(Client uc)
        {
            var units = new[]
            { /*CandleUnit.U240, */CandleUnit.M60, CandleUnit.M30, CandleUnit.M15, CandleUnit.M10, CandleUnit.M3, CandleUnit.M1 };

            var totalHours = _set.Hours;
            var hours = 3m;
            var numTest = (int)(totalHours / hours);

            info($"Entering {nameof(run_Units_FindK_3Hours)}()...");
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
                    var x = IFindK.findK(models, count * i, count, _set.ApplyMovingAvg);
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

            //
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

        FindRes runFindK(Client uc, CandleUnit unit, int count)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"--- Finding K: {count} {unit} ----");

            var models = prepareModels(uc, unit, count);

            var max = IFindK.findK(models, 0, count, _set.ApplyMovingAvg, sb);
            max.rate = Math.Round((max.rate - 1) * 100, 2);
            sb.AppendLine("---------------------------------------------------");
            sb.AppendLine($"{max.k,6:N2}: {max.rate,10:N2}%, {max.mdd,10:N2}%");
            info(sb);
            return max;
        }

        #endregion


        #region ---- BackTest ----

        void runBackTest(Client uc, CandleUnit unit, int count, decimal k)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"-------- [ Backtest: {unit}, k={k:F1}, ma={(_set.ApplyMovingAvg ? _set.MovingAvgSize : 1)} ]--------");

            var models = prepareModels(uc, unit, count);

            var (numTrades, finalRate, mdd) = IBackTest.backTest(models, k, 0, count, _set.ApplyMovingAvg);

            if (_set.PrintCandle)
            {
                sb.AppendLine("------------------------------------------------")
                    .Append($"Trades= {numTrades}/{count},")
                    .AppendLine($" Profit Rate= {(finalRate - 1) * 100:F2}%, MDD= {mdd:F2}%")
                    .AppendLine("------------------------------------------------");
            }
            info(sb);
            //File.WriteAllText($"backtest_{unit}_{k}.txt", res);
        }


        #endregion


        #region ---- Save/Load ----

        CandleModel[] prepareModels(Client uc, CandleUnit unit, int count)
        {
            var models = load(unit);
            if (models.Length == 0)
            {
                models = uc.ApiCandle<CandleMinute>(count: count, unit: unit).ToModels();
                save(models, unit);
            }
            return models;
        }

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
