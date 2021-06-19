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

    public class TickWorker : TradeWorkerBase<TickWorker, TickWorkerOptions>
    {
        readonly TimeModelQueue<TickerModel> _tickerQ;
        public TickWorker(IServiceProvider sp, string id = "") : base(sp, id)
        {
            _tickerQ = new(600);

            updateClient();
            onOptionsUpdate += updateClient;
            registerHotkey(ConsoleKey.F9, m => updateClient());
            void updateClient()
            {
                info($"[{Id}] Pausing= {_set.Pausing = !_set.Pausing}");
                _client.Pause(_set.Pausing);
            }

            registerHotkey(ConsoleKey.Spacebar, m => run_Ticker());
            registerHotkey(ConsoleKey.F1, m => changeBuy(-0.1m));
            registerHotkey(ConsoleKey.F2, m => changeBuy(+0.1m));
            registerHotkey(ConsoleKey.F3, m => changeSell(-0.1m));
            registerHotkey(ConsoleKey.F4, m => changeSell(+0.1m));
            void changeBuy(decimal delta)
            {
                _set.CalcParam.BuyMacd += delta;
                run_Ticker();
            }
            void changeSell(decimal delta)
            {
                _set.CalcParam.SellMacd += delta;
                run_Ticker();
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
                    _client.OnWsReceived += onWsReceivedTicker;

                    var request = _client.CreateInstance<IWsRequest>();
                    //request.AddTrade(CurrencyId.KRW, CoinId.BTC);
                    //request.AddOrderbook(CurrencyId.KRW, CoinId.BTC);
                    request.AddTicker(CurrencyId.KRW, CoinId.BTC);
                    await _client.ConnectWsAsync(request);
                }
                finally
                {
                    _client.OnWsReceived -= onWsReceived;
                    _client.OnWsReceived -= onWsReceivedTicker;
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
            void onWsReceivedTicker(string json)
            {
                _tickerQ.Add(_client.Deserialize<ITicker>(json).ToModel());
            }
        }

        void run_Ticker()
        {
            var param = _set.CalcParam;
            var sb = new StringBuilder();
            sb.AppendLine($"---- [Ticker] buy= {param.BuyMacd}, sell= {param.SellMacd} ----");

            var tickers = _tickerQ.ToArray();

            ICalcTicker.CalcMovingAvg(tickers, param);
            ICalcTicker.CalcMacdOsc(tickers, param);

            ICalcTicker.CalcProfitRate(tickers, param);
            var fpr = (ICalcTicker.CalcCumRate(tickers) - 1) * 100;
            //ICalcTradeTick.CalcDrawDown(ticks);

            save(tickers);
            var printTicks = tickers.Where(x => x.Signal == TimingSignal.DoBuy || x.Signal == TimingSignal.DoSell);
            print(sb, printTicks, $"final profit rate: {fpr,6:F2}%");
        }
        void run_Tick()
        {
            var param = _set.CalcParam;
            var sb = new StringBuilder();
            sb.AppendLine($"---- buy= {param.BuyMacd}, sell= {param.SellMacd} ----");

            var ticks = _client.ApiTicks(count: 500).ToModels();

            ICalcTradeTick.CalcMovingAvg(ticks, param);
            ICalcTradeTick.CalcMacdOsc(ticks, param);

            ICalcTradeTick.CalcProfitRate(ticks, param);
            var fpr = (ICalcTradeTick.CalcCumRate(ticks) - 1) * 100;
            //ICalcTradeTick.CalcDrawDown(ticks);

            save(ticks);
            var printTicks = ticks.Where(x => x.Signal == TimingSignal.DoBuy || x.Signal == TimingSignal.DoSell);
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
