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
using Universe.Coin.TradeLogic.Model;
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Calc;

namespace Universe.Coin.App
{
    public class TraderWorker : WorkerBase<TraderWorker, TraderWorkerOptions>
    {
        public TraderWorker(IServiceProvider sp, string id = "") : base(sp)
        {
            _evPausing = new(false);
            onOptionsUpdate += () => { if (!_set.Pausing) _evPausing.Set(); };

            var iw = _sp.GetRequiredService<InputWorker>();
            iw.AddCmd(ConsoleKey.F10, m =>
            {
                info($"Pausing= {_set.Pausing = !_set.Pausing}");
                if (_set.Pausing) _evPausing.Reset();
                else _evPausing.Set();
            });
        }

        protected override void work()
        {
            var logger = _sp.GetRequiredService<ILogger<IClient>>();
            var uc = new Upbit.Client(_set.AccessKey, _set.SecretKey, logger);

            //market(uc);
            //account(uc);
            ticker(uc);
            //candleDay(uc);
            //candleMinutes(uc);
            //ticks(uc);
            //orderbook(uc);

            runAutoTrade(uc);
        }

        readonly ManualResetEvent _evPausing;
        void runAutoTrade(IClient uc)
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
                if (_set.Pausing) _evPausing.WaitOne();

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
                    foreach (var tick in newTicks) report(tick, tick.Dir == TradeTickDir.U ? 1 : -1);
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

            (DateTime next, DateTime sell, decimal target) restart(IClient uc)
            {
                var models = uc.ApiCandle<CandleDay>(unit: CandleUnit.DAY, count: 2).ToModels();
                var start = models[1].TimeKST;
                info($"Starting new period: {start}");

                var next = start.AddDays(1);
                var sell = next.AddSeconds(-60);
                var target = models[1].Opening + models[0].Delta;

                return (next, sell, target);
            }
        }

        void market(IClient uc)
        {
            var markets = uc.ApiMarketInfo();
            info(IApiModel.Print(markets));
        }
        void account(IClient uc)
        {
            var accounts = uc.ApiAccount();
            info(IApiModel.Print(accounts));
            var krw = uc.GetBalance(CurrencyId.KRW);//get balnace
            var btc = uc.GetBalance(CoinId.BTC);//get btc
            info($"KRW= {krw}, BTC={btc}");
        }
        void ticker(IClient uc)
        {
            var markets = new[] {
                (CurrencyId.KRW, CoinId.BTC),
                (CurrencyId.KRW, CoinId.ETH),
                (CurrencyId.KRW, CoinId.DOGE)
            };
            var ticker = uc.ApiTicker(markets).ToModels();
            info(IViewModel.Print(ticker));
        }
        void candleDay(IClient uc)
        {
            var candles = uc.ApiCandle<CandleDay>(unit: CandleUnit.DAY, count: 20);
            var models = candles.ToModels();
            info(IViewModel.Print(models));
        }
        void candleMinutes(IClient uc)
        {
            var candles = uc.ApiCandle<CandleMinute>(unit: CandleUnit.M1, count: 20);
            var models = candles.ToModels();
            info(IViewModel.Print(models));
        }
        void orderbook(IClient uc)
        {
            var order = uc.ApiOrderbook().ToModel();
            info(order);
        }
        void ticks(IClient uc)
        {
            var ticks = uc.ApiTicks(count: 10).ToModels();
            ICalc.CalcMovingAvg(ticks, 0, ticks.Length, _set.CalcParam, m => m.UnitPrice);
            info(IViewModel.Print(ticks));
        }
    }//class
}
