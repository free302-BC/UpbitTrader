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
using Universe.AppBase.Logging;
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
        public TickWorker(IServiceProvider sp, string id = "") : base(sp, id)
        {
            _tickQ = new(600);

            updateClient();
            onOptionsUpdate += updateClient;

            registerHotkey(ConsoleKey.F9, m => updateClient());
            void updateClient()
            {
                info($"[{Id}] Pausing= {_set.Pausing = !_set.Pausing}");
                _maxWindowSize = Math.Max(_set.CalcParam.WindowSize, ((int[])_set.CalcParam.MacdParam).Max());
                _client.Pause(_set.Pausing);
            }

            registerHotkey(ConsoleKey.Spacebar, m => changeBuy(0));
            registerHotkey(ConsoleKey.F1, m => changeBuy(-0.1m));
            registerHotkey(ConsoleKey.F2, m => changeBuy(+0.1m));
            registerHotkey(ConsoleKey.F3, m => changeSell(-0.1m));
            registerHotkey(ConsoleKey.F4, m => changeSell(+0.1m));
            void changeBuy(decimal delta)
            {
                _set.CalcParam.BuyMacd += delta;
                info($"Buy={_set.CalcParam.BuyMacd}, Sell={_set.CalcParam.SellMacd}, MacdParam={_set.CalcParam.MacdParam}");
                //addTicker();
            }
            void changeSell(decimal delta)
            {
                _set.CalcParam.SellMacd += delta;
                info($"Buy={_set.CalcParam.BuyMacd}, Sell={_set.CalcParam.SellMacd}, MacdParam={_set.CalcParam.MacdParam}");
                //addTicker();
            }

            info("\r\nPress <F5> to excute run_Tick()\r\n");
            registerHotkey(ConsoleKey.F5, m => run_Tick());
        }

        protected override async Task doWork(CancellationToken stoppingToken)
        {
            await runWsTicker();

            async Task runWsTicker()
            {
                try
                {
                    _client.OnWsReceived += onWsReceived;

                    var request = _client.CreateInstance<IWsRequest>();
                    //request.AddTrade(CurrencyId.KRW, CoinId.BTC);
                    //request.AddOrderbook(CurrencyId.KRW, CoinId.BTC);
                    request.AddTicker(CurrencyId.KRW, CoinId.BTC);
                    await _client.ConnectWsAsync(request, stoppingToken);
                }
                finally
                {
                    _client.OnWsReceived -= onWsReceived;
                }
            }            
        }
        void onWsReceived(string json)
        {
            var resType = _client.GetImplType<IWsResponse>();
            var @event = ((IWsResponse)JS.Deserialize(json, resType, _jsonOptions)!).Event;

            ICalcModel model = @event switch
            {
                TradeEvent.Trade => _client.Deserialize<ITradeTick>(json).ToModel(),
                TradeEvent.Ticker => _client.Deserialize<ITicker>(json).ToModel(),
                //TradeEvent.Order => _client.Deserialize<IOrderbook>(json).ToModel(),
                _ => throw new NotImplementedException(),
            };

            //if (model is TradeTickModel) report(model, (int)((TradeTickModel)model).Dir);
            //else report(model);

            if (@event == TradeEvent.Trade) _tradeCounter++;
            else if (@event == TradeEvent.Ticker) _tickerCounter++;

            _tickQ.Add(model);
            run(model);
        }

        volatile int _maxWindowSize = 0;
        volatile int _tradeCounter = 0;
        volatile int _tickerCounter = 0;
        void run(ICalcModel model)
        {
            var param = _set.CalcParam;
            var models = _tickQ.Last(_maxWindowSize);
            if (models.Length == 0) return;

            ICalc.CalcMovingAvg(models, param, models.Length - 1);
            ICalc.CalcMacd(models, param, models.Length - 1);
            TickerCalc.I.CalcProfitRate(models, param, models.Length - 1);
            ICalc.CalcCumRate(models, models.Length - 1);
            //ICalcTradeTick.CalcDrawDown(ticks);

            report($"{model.ToCalcString()} | ({_tradeCounter}, {_tickerCounter})", (int)model.Signal);
            //File.WriteAllText("ticker_ws.txt", model.ToCalcString());
        }

        void run_Tick()
        {
            var param = _set.CalcParam;
            var sb = new StringBuilder();
            sb.AppendLine($"---- buy= {param.BuyMacd}, sell= {param.SellMacd} ----");

            var ticks = _client.ApiTicks(count: 200000).ToModels();

            ICalc.CalcMovingAvg(ticks, param);
            ICalc.CalcMacd(ticks, param);

            TickerCalc.I.CalcProfitRate(ticks, param);
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
            foreach (var m in models) sb.AppendLine(m.ToCalcString());
            sb.AppendLine("----------------------------------------");
            sb.AppendLine(epilog);
            info(sb);
        }

    }//class
}
