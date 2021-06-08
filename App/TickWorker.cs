using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Universe.AppBase;
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Calc;
using Universe.Coin.TradeLogic.Model;
using Universe.Coin.Upbit.Model;

namespace Universe.Coin.Upbit.App
{
    using JS = System.Text.Json.JsonSerializer;

    public class TickWorker : WorkerBase<TickWorker, BackTestOptions>
    {
        readonly Client _client;
        readonly JsonSerializerOptions _jsonOptions;

        public TickWorker(
            ILogger<TickWorker> logger,
            IServiceProvider sp,
            IOptionsMonitor<BackTestOptions> set)
            : base(logger, sp, set)
        {
            _jsonOptions = Helper.GetJsonOptions();
            _client = new Client(_set.AccessKey, _set.SecretKey, _sp.GetRequiredService<ILogger<Client>>());
            updateClient();

            onOptionsUpdate += updateClient;

            var iw = _sp.GetRequiredService<InputWorker>();
            iw.AddCmd(ConsoleKey.F9, m => updateClient());

            void updateClient()
            {
                info($"Pausing= {_set.Pausing = !_set.Pausing}");
                _client.Pause(_set.Pausing);
            }
        }
        protected override void work()
        {
            //run_Tick_K(_client);

            var request = new WsRequest();
            //request.Add("orderbook", Helper.GetMarketId(CurrencyId.KRW, CoinId.BTC));

            _client.OnReceived += uc_OnReceived;
            _client.ConnectWs(request);
        }

        private void uc_OnReceived(string json)
        {
            var type = JS.Deserialize<WsResponse>(json, _jsonOptions)!.requestType;//TODO: !

            if (type == "trade")
            {
                var model = JS.Deserialize<TradeTick>(json, _jsonOptions)?.ToModel() ?? new();
                report(model, (int)model.Dir);
            }
            if (type == "ticker")
            {
                var model = JS.Deserialize<Ticker>(json, _jsonOptions)?.ToModel() ?? new();
                report(model);
            }
            if (type == "orderbook")
            {
                var model = JS.Deserialize<Orderbook>(json, _jsonOptions)?.ToModel() ?? new();
                report(model);
            }
        }

        void run_Tick_K(Client uc)
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
