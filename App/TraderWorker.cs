using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Universe.AppBase;
using System.Text.Json;
using Universe.Coin.Upbit.Model;
using System.IO;
using System.Collections.Specialized;
using Universe.Utility;

namespace Universe.Coin.Upbit.App
{
    public class TraderWorker : WorkerBase<TraderWorker, WorkerSetting>
    {
        public TraderWorker(ILogger<TraderWorker> logger, IOptionsMonitor<WorkerSetting> set, IServiceProvider sp)
            : base(logger, set, sp) { }

        protected override void work(WorkerSetting set)
        {
            var uc = new Client(set.AccessKey, set.SecretKey, _sp.GetRequiredService<ILogger<Client>>());
            try
            {
                //market(uc);
                //account(uc);
                //ticker(uc);
                //candleDay(uc);
                candleMinutes(uc);
                //orderbook(uc);
                //ticks(uc);

                //run(uc);
            }
            catch (Exception ex)
            {
                log("work():", ex.Message);
            }
        }

        void run(Client uc)
        {
            var (next, sell, target) = restart(uc);

            bool buy = false;
            var lastTicks = new List<long>();
            var lastOrder = new OrderbookModel();
            var numTicks = 50;
            var deviation = 0;
            info($"deviation= {deviation}, numTicks= {numTicks}");
            var lastBook = DateTime.Now;

            while (true)
            {
                Thread.Sleep(90);
                var now = DateTime.Now;
                if (now < sell)
                {
                    var order = uc.ApiOrderbook().ToModel();//1st call-price
                    if ((now - lastBook).TotalSeconds > 1)
                    {
                        //report call-price
                        if (lastOrder != order) report(order);
                        lastOrder = order;
                        lastBook = now;
                    }

                    //get new ticks
                    var newTicks = uc.ApiTicks(count: numTicks).ToModels().Where(x => !lastTicks.Contains(x.Serial));
                    var numNewTicks = newTicks.Count();
                    if (numNewTicks > numTicks / 5) info($"numNewTicks= <{numNewTicks}>");
                    foreach (var tick in newTicks) report(tick, tick.Dir == "▲" ? 1 : -1);
                    lastTicks.AddRange(newTicks.Select(x => x.Serial));

                    //check ticks buffer
                    if (lastTicks.Count > 5 * numTicks)
                    {
                        var num0 = lastTicks.Count;
                        lastTicks.RemoveRange(0, num0 - 3 * numTicks);
                        info($"lastTicks full: {num0} -> {lastTicks.Count}, numTicsk={numTicks}");
                        deviation++;
                    }

                    //update numTicks
                    if (deviation < 0 && numNewTicks > numTicks / 2)
                    {
                        info($"numNewTicks= {numNewTicks}, numTicks -> {numTicks *= 2} ▲");
                        deviation = 0;
                    }
                    else if (numNewTicks < numTicks / 10) deviation--;
                    if (deviation < -100)
                    {
                        if (numTicks > 10) info($"deviation= {deviation}, numTicks -> {numTicks = numTicks * 10 / 11} ▼");
                        deviation = 0;
                    }

                    //check buy condition
                    var current = order.askUP;
                    if (current > target && !buy)
                    {
                        var krw = uc.GetBalance(CurrencyId.KRW);//get balnace
                        if (krw > 5000)
                        {
                            ;//buy 
                        }
                        buy = true;//test
                        info($"=======> Buying KRW_BTC: {krw}KRW <=========");
                    }
                }
                else if (sell <= now && now < next)
                {
                    var btc = uc.GetBalance(CoinId.BTC);//get btc
                    if (btc > 0.0001m)
                    {
                        ;//sell
                    }
                    buy = false;//test
                    info($"=======> Selling KRW_BTC: {btc}BTC <=========");
                    Thread.Sleep(60000);
                }
                else//now >= next
                {
                    (next, sell, target) = restart(uc);
                }
            }//while
        }
        (DateTime next, DateTime sell, decimal target) restart(Client uc)
        {
            var models = uc.ApiCandle<CandleDay>(count: 2).ToModels();
            var start = models[1].TimeKST;
            info($"Starting new period: {start}");
            var next = start.AddDays(1);
            var sell = next.AddSeconds(-60);

            CandleModel.CalcRate(models, 0.5m);
            var target = models[1].Target;

            return (next, sell, target);
        }

        void market(Client uc)
        {
            var markets = uc.ApiMarketInfo();
            info(IApiModel.Print(markets));
        }
        void account(Client uc)
        {
            var accounts = uc.ApiAccount();
            info(IApiModel.Print(accounts));
            var krw = uc.GetBalance(CurrencyId.KRW);//get balnace
            var btc = uc.GetBalance(CoinId.BTC);//get btc
            info($"KRW= {krw}, BTC={btc}");
        }
        void ticker(Client uc)
        {
            var markets = new[] {
                (CurrencyId.KRW, CoinId.BTC),
                (CurrencyId.KRW, CoinId.ETH),
                (CurrencyId.KRW, CoinId.DOGE)
            };
            var ticker = uc.ApiTicker(markets).ToModels();
            info(IViewModel.Print(ticker));
        }
        void candleDay(Client uc)
        {
            var candles = uc.ApiCandle<CandleDay>(count: 10);
            var models = candles.ToModels();
            info(IViewModel.Print(models));
        }
        void candleMinutes(Client uc)
        {
            var candles = uc.ApiCandle<CandleMinute>(count: 10, unit: CandleUnit.U60);
            var models = candles.ToModels();
            info(IViewModel.Print(models));
        }
        void orderbook(Client uc)
        {
            var order = new OrderbookModel(uc.ApiOrderbook());//call price
            info(order);
        }
        void ticks(Client uc)
        {
            var ticks = uc.ApiTicks(count: 10).ToModels();
            info(IViewModel.Print(ticks));
        }
    }//class
}
