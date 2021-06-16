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

            var request = _client.CreateInstance<IWsRequest>();
            request.AddTrade(CurrencyId.KRW, CoinId.BTC);
            request.AddOrderbook(CurrencyId.KRW, CoinId.BTC);
            _client.ConnectWs(request);
        }

        private void uc_OnReceived(string json)
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

            if(model is TradeTickModel) report(model, (int)((TradeTickModel)model).Dir);
            else report(model);
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
