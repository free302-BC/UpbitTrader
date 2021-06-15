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

using Universe.Coin.Upbit.Model;
using Universe.Utility;

namespace Universe.Coin.App
{
    using JS = JsonSerializer;

    public class TickWorker : TradeWorkerBase<TickWorker, TickWorkerOptions>
    {
        public TickWorker(IServiceProvider sp, string id = "") : base(sp, id)
        {
            updateClient();
            onOptionsUpdate += updateClient;
            registerHotkey(ConsoleKey.F9, m => updateClient());

            void updateClient()
            {
                info($"[{Id}] Pausing= {_set.Pausing = !_set.Pausing}");
                _client.Pause(_set.Pausing);
            }
        }
                
        protected override void work()
        {
            //run_Tick_K(_client);

            _client.OnWsReceived += uc_OnReceived;

            var request = UvLoader.Create<IWsRequest>(_set.AssemblyFile, "Universe.Coin.Upbit.WsRequest");
            request.AddTrade(CurrencyId.KRW, CoinId.BTC);
            request.AddOrderbook(CurrencyId.KRW, CoinId.BTC);
            _client.ConnectWs(request);
        }

        private void uc_OnReceived(string json)
        {
            var type = UvLoader.Create<IWsResponse>(_set.AssemblyFile, "Universe.Coin.Upbit.Model.WsResponse").GetType();
            //var @event = JS.Deserialize<IWsResponse>(json, _jsonOptions)!.Event;
            var @event = ((IWsResponse)JS.Deserialize(json, type, _jsonOptions)!).Event;

            if (@event == TradeEvent.Trade)
            {
                var model = JS.Deserialize<TradeTick>(json, _jsonOptions)?.ToModel() ?? new();
                report(model, (int)model.Dir);
            }
            if (@event == TradeEvent.Ticker)
            {
                var model = JS.Deserialize<Ticker>(json, _jsonOptions)?.ToModel() ?? new();
                report(model);
            }
            if (@event == TradeEvent.Order)
            {
                var model = JS.Deserialize<Orderbook>(json, _jsonOptions)?.ToModel() ?? new();
                report(model);
            }
        }

        void run_Tick_K(IClient uc)
        {
            var param = _set.CalcParam;
            var ticks = uc.ApiTicks(count: 10).ToModels();
            ICalcTradeTick.CalcMovingAvg(ticks, param);
            ICalcTradeTick.CalcMacdOsc(ticks, param);
            //ICalcTradeTick.CalcProfitRate(ticks, param);
            //ICalcTradeTick.CalcCumRate(ticks);
            //ICalcTradeTick.CalcDrawDown(ticks);

            info(IViewModel.Print(ticks));
        }


    }//class
}
