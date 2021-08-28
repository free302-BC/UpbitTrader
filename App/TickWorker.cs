using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Universe.AppBase;
using Universe.Logging;
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Calc;
using Universe.Coin.TradeLogic.Model;
using Universe.Utility;

namespace Universe.Coin.App
{
    using JS = JsonSerializer;

    public class TickWorker : TradeWorkerBase<TickWorker, TickWorkerOptions>
    {
        readonly TimeModelQueue<ICalcModel> _tickQ;
        volatile int _maxWindowSize = 0;
        string _headerLine = "";

        public TickWorker(IServiceProvider sp, string id = "") : base(sp, id)
        {
            _tickQ = new(600);

            updateClient();
            onOptionsUpdate += updateClient;

            registerHotkey(ConsoleKey.F9, () => 
            {
                _set.Pausing = !_set.Pausing;
                _client.Pause(_set.Pausing);
                updateClient(); 
            });
            void updateClient()
            {
                _maxWindowSize = Math.Max(_set.CalcParam.WindowSize, ((int[])_set.CalcParam.MacdParam).Max());
                printParam();
            }

            registerHotkey(ConsoleKey.Spacebar, () => printParam());
            registerHotkey(ConsoleKey.F1, () => changeParam(-0.1m, true));
            registerHotkey(ConsoleKey.F2, () => changeParam(+0.1m, true));
            registerHotkey(ConsoleKey.F3, () => changeParam(-0.1m, false));
            registerHotkey(ConsoleKey.F4, () => changeParam(+0.1m, false));
            void changeParam(decimal delta, bool buy)
            {
                if(buy) _set.CalcParam.BuyMacd += delta;
                else _set.CalcParam.SellMacd += delta;
                printParam();
            }            
            void printParam()
            {
                info($"[{Id}] Pausing={_set.Pausing}, Buy={_set.CalcParam.BuyMacd}, Sell={_set.CalcParam.SellMacd}, MacdParam={_set.CalcParam.MacdParam}");
                report(_headerLine);
            }

            info("Press <F5> to excute run_Tick()");
            registerHotkey(ConsoleKey.F5, () => run_Tick());
        }

        protected override async Task doWork(CancellationToken stoppingToken)
        {
            try
            {
                //event proxy 수행
                _ = base.doWork(stoppingToken);

                _client.WsTick += onWsTick;

                _client.AddTick(CurrencyId.KRW, CoinId.BTC);
                _headerLine = TickModel.Empty.CalcHeader;

                await _client.ConnectWsAsync(stoppingToken);
            }
            finally
            {
                _client.WsTick -= onWsTick;
            }
        }

        void onWsTick(TickModel model)
        {
            _tickQ.Add(model);
            run(model);
        }        

        void run(ICalcModel model)
        {
            var param = _set.CalcParam;
            var models = _tickQ.Last(_maxWindowSize);
            if (models.Length == 0) return;

            ICalc.CalcMovingAvg(models, param, models.Length - 1);
            ICalc.CalcMacd(models, param, models.Length - 1);
            TickCalc.I.CalcABR(models, models.Length - 1);
            TickCalc.I.CalcProfitRate(models, param, models.Length - 1);
            ICalc.CalcCumRate(models, models.Length - 1);
            //ICalcTradeTick.CalcDrawDown(ticks);

            report(model.ToCalcString(), (int)model.Signal);
            //File.WriteAllText("ticker_ws.txt", model.ToCalcString());
        }

        void run_Tick()
        {
            var param = _set.CalcParam;
            var sb = new StringBuilder();
            var count = 720;
            info($"---- count={count}, buy= {param.BuyMacd}, sell= {param.SellMacd} ----");

            var ticks = _client.ApiTicks(count: count).ToModels();

            ICalc.CalcMovingAvg(ticks, param);
            ICalc.CalcMacd(ticks, param);
            TickCalc.I.CalcABR(ticks);
            TickCalc.I.CalcProfitRate(ticks, param);
            var fpr = (ICalc.CalcCumRate(ticks) - 1) * 100;
            //ICalcTradeTick.CalcDrawDown(ticks);

            save(ticks);
            var printTicks = ticks.Where(x => x.Signal == TimingSignal.Buy || x.Signal == TimingSignal.Sell);
            print(sb, printTicks, $"final profit rate: {fpr,6:F2}%");
        }
        static void save(ICalcModel[] models)
        {
            var sb = new StringBuilder();
            sb.AppendLine(models[0].CalcHeader);
            foreach (var m in models) sb.AppendLine(m.ToCalcString());
            File.WriteAllText("tick_test.txt", sb.ToString());
        }
        void print(StringBuilder sb, IEnumerable<ICalcModel> models, string epilog = "")
        {
            sb.AppendLine();
            foreach (var m in models) sb.AppendLine(m.ToCalcString());
            sb.AppendLine("----------------------------------------");
            sb.AppendLine(epilog);
            info(sb);
        }

    }//class
}
