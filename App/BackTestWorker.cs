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

            while (true)
            {
                if (!_doFindK)
                {
                    var wf = _set.WindowFunction;
                    _set.WindowFunction = WindowFunction.None;
                    run_Units_K(uc);
                    _set.WindowFunction = wf;
                    run_Units_K(uc);
                }
                else
                {
                    var wf = _set.WindowFunction;
                    _set.WindowFunction = WindowFunction.None;
                    run_Units_FindK(uc);
                    _set.WindowFunction = wf;
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
                iw.AddCmd(ConsoleKey.Spacebar, (m) => ev.Set());
                iw.AddCmd(ConsoleKey.Enter, (m) => ev.Set());
                iw.AddCmd(ConsoleKey.F1, onChangeTest);
                iw.AddCmd(ConsoleKey.F2, m => onChangeNumericParam(m, 3, () => _set.Hours));
                iw.AddCmd(ConsoleKey.F3, m => onChangeNumericParam(m, 0.1m, () => _set.FactorK));
                iw.AddCmd(ConsoleKey.F4, m => onChangeNumericParam(m, 3, () => _set.WindowSize));
                iw.AddCmd(ConsoleKey.F5, onToggleWF);
                
                void onChangeTest(ConsoleModifiers modifier)
                {
                    _doFindK = !_doFindK;
                    _ev.Set();
                }
                void onChangeNumericParam<P>(ConsoleModifiers modifier, P delta, Expression<Func<P>> selector)
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
                    info($"Reloaded: k={_set.FactorK}, hours={_set.Hours}, ma={_set.WindowSize}");
                    ev.Set();
                }
                void onToggleWF(ConsoleModifiers modifier)
                {
                    var delta = modifier.HasFlag(ConsoleModifiers.Shift) ? -1 : +1;
                    _set.WindowFunction = (WindowFunction)(((int)_set.WindowFunction + delta) % 3);
                    _ev.Set();
                }
            }
        }


        /// <summary>
        /// 단일 k에 대하여 각 CandleUnit별로 backtest 수행
        /// </summary>
        /// <param name="uc"></param>
        void run_Units_K(Client uc)
        {
            var units = new[]
            { CandleUnit.M240, CandleUnit.M60, CandleUnit.M30, CandleUnit.M15, CandleUnit.M10, CandleUnit.M3, CandleUnit.M1 };
            var results = new BtList();
            var sb = new StringBuilder();
            var hours = _set.Hours;
            var k = _set.FactorK;
            var ma = (_set.WindowFunction, _set.WindowSize);

            info($"Entering {nameof(run_Units_K)}()...");
            sb.AppendLine($"-----------[ {(int)(hours / 24)}d {hours % 24}h : k={k:F1} ma={ma} ]------------");

            foreach (var unit in units)
            {
                var count = (int)(hours * 60 / (int)unit);
                var models = prepareModels(uc, unit, count);

                var x = IBackTest.BackTest(models, 0, count, _set);
                results.Add((unit, count, x));
                sb.AppendLine($"{count,6} {unit,6}: {k,6:F2} {x.rate,8:F4} {x.mdd,6:F2}%");
            }
            var maxRate = results.Max(x => x.res.rate);
            var max = results.First(x => x.res.rate == maxRate);

            sb.AppendLine("-------------------------------------------");
            sb.Append($"{max.count,6} {max.unit,6}: {k,6:F2} {(max.res.rate - 1) * 100,7:F2}% {max.res.mdd,6:F2}%");
            sb.AppendLine($" | {max.res.trades}/{max.count}Tr");
            sb.AppendLine("-------------------------------------------");

            if (_set.PrintCandle) runBackTest(uc, max.unit, max.count, sb);
            info(sb);
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
            var ma = (_set.WindowFunction, _set.WindowSize);

            info($"Entering {nameof(run_Units_FindK)}()...");
            sb.AppendLine($"-----------[ {(int)(hours / 24)}d {hours % 24}h : ma={ma} ]------------");

            foreach (var unit in units)
            {
                var count = (int)(hours * 60 / (int)unit);
                var models = prepareModels(uc, unit, count);

                var x = IFindK.FindK(models, 0, count, _set);
                results.Add((unit, count, x));
                sb.AppendLine($"{count,6} {unit,6}: {x.k,6:F2} {x.rate,8:F4} {x.mdd,6:F2}%");
            }
            var maxRate = results.Max(x => x.res.rate);
            var max = results.First(x => x.res.rate == maxRate);

            sb.AppendLine("-------------------------------------------");
            sb.Append($"{max.count,6} {max.unit,6}: {max.res.k,6:F2} {(max.res.rate - 1) * 100,6:F2}%, {max.res.mdd,6:F2}%");
            sb.AppendLine($" | {max.res.trades}/{max.count}Tr");
            sb.AppendLine("-------------------------------------------");

            if (_set.PrintCandle) runBackTest(uc, max.unit, max.count, sb);
            info(sb.ToString());
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
                    var x = IFindK.FindK(models, count * i, count, _set);
                    results.Add(x);
                    //sb.Append($"{(x.rate - 1) * 100,6:N2} ");
                }
                kDic.Add(unit, results);

                //sb.AppendLine("-------------------------------------------");
                // 누적 구하기 ~ 중복 문제 주의
                //var cumRate0 = results.Aggregate(1m, (p, x) => p * x.rate);//decimal overflow error ~ 10^40 order
                var cumRateInteger = results.Aggregate(BigInteger.One, (p, x) => p * (int)(10000 * x.rate));
                var cumRate = cumRateInteger / BigInteger.Pow(10000, results.Count - 1) - 10000;// %% := % * 100 (%를 정수로 표현)
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


        #region ---- BackTest ----

        void runBackTest(Client uc, CandleUnit unit, int count, StringBuilder? sb = null)
        {
            var models = prepareModels(uc, unit, count);
            var (numTrades, finalRate, mdd) = IBackTest.BackTest(models, 0, count, _set);

            bool doPrint = sb == null;
            sb = sb ?? new StringBuilder();

            var k = _set.FactorK;
            var ma = (_set.WindowFunction, _set.WindowSize);
            sb.AppendLine($"-------- [ Backtest: {unit}, k={k:F1}, ma={ma} ]--------");
            if (_set.PrintCandle) sb.Append(models.Where(x => x.Rate != 1m && x.Rate != 0m).Print());
            sb.AppendLine("-------------------------------------------");
            sb.Append($"{count,6} {unit,6}: {k,6:F2} {(finalRate - 1) * 100,7:F2}% {mdd,6:F2}%");
            sb.AppendLine($" | {numTrades}/{count}Tr");
            sb.AppendLine("-------------------------------------------");

            if (doPrint) info(sb);
            //File.WriteAllText($"backtest_{unit}_{k}.txt", res);
        }


        #endregion


        #region ---- Save/Load ----

        CandleModel[] prepareModels(Client uc, CandleUnit unit, int count)
        {
            CandleModel[] models = _set.LoadFromFile ? load(unit) : Array.Empty<CandleModel>();
            if (models.Length < count)
            {
                models = uc.ApiCandle<CandleMinute>(count: count, unit: unit).ToModels();
                save(models, unit);
            }
            return models!;
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
