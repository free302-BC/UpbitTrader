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
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Calc;
using Universe.Coin.TradeLogic.Model;
using Universe.Utility;

namespace Universe.Coin.App
{
    using JS = JsonSerializer;

    public class TickerWorker : TradeWorkerBase<TickerWorker, TickerWorkerOptions>
    {
        readonly TimeModelQueue<TickerModel> _tickerQ;
        public TickerWorker(IServiceProvider sp, string id = "") : base(sp, id)
        {
            _tickerQ = new(600);

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
        }

        protected override void doWork()
        {
            runWsTicker();

            async void runWsTicker()
            {
                try
                {
                    //_client.OnWsReceived += onWsReceived;
                    _client.OnWsReceived += addTicker;

                    var request = _client.CreateInstance<IWsRequest>();
                    //request.AddTrade(CurrencyId.KRW, CoinId.BTC);
                    //request.AddOrderbook(CurrencyId.KRW, CoinId.BTC);
                    request.AddTicker(CurrencyId.KRW, CoinId.BTC);
                    await _client.ConnectWsAsync(request);
                }
                finally
                {
                    _client.OnWsReceived -= onWsReceived;
                    _client.OnWsReceived -= addTicker;
                }
            }
            void onWsReceived(string json)
            {
                var resType = _client.GetImplType<IWsResponse>();
                var @event = ((IWsResponse)JS.Deserialize(json, resType, _jsonOptions)!).Event;

                IViewModel model = @event switch
                {
                    TradeEvent.Trade => _client.Deserialize<ITradeTick>(json).ToModel(),
                    TradeEvent.Ticker => _client.Deserialize<ITicker>(json).ToModel(),
                    TradeEvent.Order => _client.Deserialize<IOrderbook>(json).ToModel(),
                    _ => throw new NotImplementedException(),
                };

                if (model is TradeTickModel) report(model, (int)((TradeTickModel)model).Dir);
                else report(model);
            }
        }

        volatile int _maxWindowSize = 0;
        void addTicker(string json)
        {
            var model = _client.Deserialize<ITicker>(json).ToModel();
            _tickerQ.Add(model);

            var param = _set.CalcParam;
            var tickers = _tickerQ.Last(_maxWindowSize);
            if (tickers.Length == 0) return;

            ICalc.CalcMovingAvg(tickers, param, tickers.Length - 1);
            ICalc.CalcMacd(tickers, param, tickers.Length - 1);
            TickerCalc.I.CalcProfitRate(tickers, param, tickers.Length - 1);
            ICalc.CalcCumRate(tickers, tickers.Length - 1);
            //ICalcTradeTick.CalcDrawDown(ticks);

            report(model.ToCalcString(), (int)model.Signal);
            File.WriteAllText("ticker_ws.txt", model.ToCalcString());
        }
        void run_Tick()
        {
            var param = _set.CalcParam;
            var sb = new StringBuilder();
            sb.AppendLine($"---- buy= {param.BuyMacd}, sell= {param.SellMacd} ----");

            var ticks = _client.ApiTicks(count: 500).ToModels();

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
