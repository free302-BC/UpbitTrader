﻿using System;
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
            var uc = new Client(set.TokenFile, _sp.GetRequiredService<ILogger<Client>>());
            try
            {
                //run(uc);
                var a1 = uc.ApiAccount(); info("a1"); 
                var a2 = uc.ApiAccount(); info("a2"); 
                var a3 = uc.ApiAccount(); info("a3"); 
                var a4 = uc.ApiAccount(); info("a4"); 
                var a5 = uc.ApiAccount(); info("a5"); 
                var a6 = uc.ApiAccount(); info("a6"); 
                var a7 = uc.ApiAccount(); info("a7"); 
                var a8 = uc.ApiAccount(); info("a8"); 
                var a9 = uc.ApiAccount(); info("a9"); 
                var a10 = uc.ApiAccount(); info("a10");
            }
            catch (Exception e)
            {
                log("work():", e.Message);
            }
        }
        void run(Client uc)
        {
            var (next, sell, target) = restart(uc);

            bool buy = false;
            while (true)
            {
                Thread.Sleep(1000);
                var now = DateTime.Now;
                if (now < sell)
                {
                    //var ticker = new CalcModel(uc.ApiTicker());
                    //info(ticker.ToTickerString());
                    var order = new CalcModel(uc.ApiOrderbook().OrderbookUnits[0]);
                    info(order.ToOrderString());
                    var current = order.Ask;
                    if (current > target && !buy)
                    {
                        var krw = 0.0;//get balnace
                        if (krw > 5000)
                        {
                            ;//buy 
                        }
                        buy = true;
                        info($"Buying KRW_BTC: {krw}KRW");
                    }
                }
                else if (sell <= now && now < next)
                {
                    var btc = 0.0;//get btc
                    if (btc > 0.00008)
                    {
                        buy = false;
                    }
                    info($"Selling KRW_BTC: {btc}BTC");
                    Thread.Sleep(60);
                }
                else//now >= next
                {
                    (next, sell, target) = restart(uc);
                }

            }//while

        }
        (DateTime next, DateTime sell, double target) restart(Client uc)
        {
            var models = uc.ApiDayModels(2);
            var start = models[1].DateKST;
            info($"Starting new period: {start}");
            var next = start.AddDays(1);
            var sell = next.AddSeconds(-60);

            CalcModel.CalcRate(models, 0.5);
            var target = models[1].Target;

            return (next, sell, target);
        }

       

    }//class
}