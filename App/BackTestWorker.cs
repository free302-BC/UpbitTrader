using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Text.Json;
using System.IO;
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Model;
using Universe.Coin.TradeLogic.Calc;
using System.Threading.Tasks;

namespace Universe.Coin.App
{
    using BtRes = ValueTuple<int, decimal, decimal>;//(trades, rate, mdd)
    using BtList = List<(CandleUnit unit, int count, (int trades, decimal rate, decimal mdd) res)>;
    using FindRes = ValueTuple<int, decimal, decimal, decimal>;//(trades, k, rate, mdd)
    using FindList = List<(CandleUnit unit, int count, (int trades, decimal k, decimal rate, decimal mdd) res)>;

    public class BackTestWorker : TradeWorkerBase<BackTestWorker, BackTestOptions>
    {
        #region ---- Ctor ----

        public BackTestWorker(IServiceProvider sp, string id = "") : base(sp, id)
        {
            _ev = new(false);
            onOptionsUpdate += () => info($"{Id} updated!");

            registerHotkey();
        }
        void registerHotkey()
        {
            registerHotkey(ConsoleKey.Spacebar, () => _ev.Set());
            registerHotkey(ConsoleKey.F1, () =>
            {
                _set.DoFindK = !_set.DoFindK;
                info($"DoFindK: {_set.DoFindK}");
            });

            registerHotkey(ConsoleKey.F2, () => _set.Hours += 3);
            registerHotkey(ConsoleKey.F2, () => _set.Hours -= 3, ConsoleModifiers.Shift);
            registerHotkey(ConsoleKey.F2, () => _set.Hours *= 2, ConsoleModifiers.Control);
            registerHotkey(ConsoleKey.F2, () => _set.Hours /= 2, ConsoleModifiers.Control | ConsoleModifiers.Shift);

            registerHotkey(ConsoleKey.F3, () => _set.CalcParam.FactorK += 0.1m);
            registerHotkey(ConsoleKey.F3, () => _set.CalcParam.FactorK -= 0.1m, ConsoleModifiers.Shift);
            registerHotkey(ConsoleKey.F3, () => _set.CalcParam.FactorK *= 2, ConsoleModifiers.Control);
            registerHotkey(ConsoleKey.F3, () => _set.CalcParam.FactorK /= 2, ConsoleModifiers.Control | ConsoleModifiers.Shift);

            registerHotkey(ConsoleKey.F4, () => _set.CalcParam.WindowSize += 3);
            registerHotkey(ConsoleKey.F4, () => _set.CalcParam.WindowSize -= 3, ConsoleModifiers.Shift);
            registerHotkey(ConsoleKey.F4, () => _set.CalcParam.WindowSize *= 2, ConsoleModifiers.Control);
            registerHotkey(ConsoleKey.F4, () => _set.CalcParam.WindowSize /= 2, ConsoleModifiers.Control | ConsoleModifiers.Shift);

            registerHotkey(ConsoleKey.F5, ()=> onToggleWF(1));
            registerHotkey(ConsoleKey.F5, () => onToggleWF(-1), ConsoleModifiers.Shift);
            registerHotkey(ConsoleKey.F12, onRemoveFile);
            
            void onToggleWF(int delta)
            {
                //var delta = modifier.HasFlag(ConsoleModifiers.Shift) ? -1 : +1;

                _set.CalcParam.WindowFunction
                    = (WindowFunction)(((int)_set.CalcParam.WindowFunction + delta) % (int)(1 + WindowFunction.Gaussian));
                info($"WindowFunction: {_set.CalcParam.WindowFunction}");
            }
            void onRemoveFile()
            {
                var units = new[]
                { CandleUnit.M240, CandleUnit.M60, CandleUnit.M30,
                        CandleUnit.M15, CandleUnit.M10, CandleUnit.M3, CandleUnit.M1 };
                foreach (var u in units) removeFile(u);
                info($"Cache files removed");
            }
        }

        public new void Dispose()
        {
            _ev?.Dispose();
            base.Dispose();
        }

        #endregion

        static (int trades, decimal k, decimal rate, decimal mdd) cast(FindRes res) => res;
        static (int trades, decimal rate, decimal mdd) cast(BtRes res) => res;

        readonly ManualResetEvent _ev;
        protected override Task doWork(CancellationToken stoppingToken)
        {
            //event proxy 수행
            _ = base.doWork(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                if (!_set.DoFindK) 
                    run_Units_K(_client);
                else
                    run_Units_FindK(_client);

                info($"<{Id}> Waiting...");
                _ev.Reset();
                _ev.WaitOne();
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 단일 k에 대하여 각 CandleUnit별로 backtest 수행
        /// </summary>
        /// <param name="uc"></param>
        void run_Units_K(IClient uc)
        {
            var results = new BtList();
            var sb = new StringBuilder();

            var units = new[] { 
                CandleUnit.M240, CandleUnit.M60, CandleUnit.M30, CandleUnit.M15, 
                CandleUnit.M10, CandleUnit.M3, CandleUnit.M1 };
            var hours = _set.Hours;
            var k = _set.CalcParam.FactorK;
            var ma = (_set.CalcParam.WindowFunction, _set.CalcParam.WindowSize);

            info($"Entering {nameof(run_Units_K)}()...");
            sb.AppendLine($"-----------[ {new TimeSpan(hours, 0, 0)} : k={k:F1} ma={ma} ]------------");

            foreach (var unit in units)
            {
                var count = (int)(hours * 60 / (int)unit);
                var models = prepareModels(uc, unit, count);

                var x = IBackTest.BackTest(models, _set.CalcParam);
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
        void run_Units_FindK(IClient uc)
        {
            var units = new[]
            { CandleUnit.M240, CandleUnit.M60, CandleUnit.M30, CandleUnit.M15, CandleUnit.M10, CandleUnit.M3, CandleUnit.M1 };
            var results = new FindList();
            var sb = new StringBuilder();
            var hours = _set.Hours;
            var ma = (_set.CalcParam.WindowFunction, _set.CalcParam.WindowSize);

            info($"Entering {nameof(run_Units_FindK)}()...");
            sb.AppendLine($"-----------[ {(int)(hours / 24)}d {hours % 24}h : ma={ma} ]------------");

            foreach (var unit in units)
            {
                var count = (int)(hours * 60 / (int)unit);
                var models = prepareModels(uc, unit, count);

                var x = IFindK.FindK(models, _set.CalcParam);
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


        #region ---- BackTest ----

        void runBackTest(IClient uc, CandleUnit unit, int count, StringBuilder? sb = null)
        {
            var models = prepareModels(uc, unit, count);
            var (numTrades, finalRate, mdd) = IBackTest.BackTest(models, _set.CalcParam);

            bool doPrint = sb == null;
            sb = sb ?? new StringBuilder();

            var k = _set.CalcParam.FactorK;
            var ma = (_set.CalcParam.WindowFunction, _set.CalcParam.WindowSize);
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

        CandleModel[] prepareModels(IClient uc, CandleUnit unit, int count)
        {
            CandleModel[] models = _set.LoadFromFile ? load(unit) : Array.Empty<CandleModel>();
            if (models.Length < count)
            {
                models = uc.ApiCandle<ICandle>(count: count, unit: unit).ToModels();
                save(models, unit);
            }
            return models.Take(count).ToArray();
        }

        void save(CandleModel[] models, CandleUnit unit)
        {
            var json = JsonSerializer.Serialize(models, _jsonOptions);
            File.WriteAllText($"{unit}.json", json);
        }
        CandleModel[] load(CandleUnit unit)
        {
            var fn = $"{unit}.json";
            if (File.Exists(fn))
            {
                var json = File.ReadAllText(fn);
                var models = JsonSerializer.Deserialize<CandleModel[]>(json, _jsonOptions);
                if (models != null) return models;
            }
            return Array.Empty<CandleModel>();
        }
        void removeFile(CandleUnit unit)
        {
            File.Delete($"{unit}.json");
        }

        #endregion


        #region ---- TEST ----

        void saveKey(TradeWorkerOptions set)
        {
            IClientOptions.SaveEncrptedKey(set.AccessKey, set.SecretKey, "key.txt");
        }


        #endregion

    }//class
}
