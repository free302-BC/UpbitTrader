using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Universe.AppBase;
using Microsoft.Extensions.Options;
using System.Threading;
using Microsoft.Extensions.Hosting;

namespace Universe.Coin.App
{
    class Program : ProgramBase
    {
        static void Main(string[] args)
        {
            #region ---- key input listener ----
            AddWorker<InputWorker, TradeWorkerOptions>(
                lifeTime: ServiceLifetime.Singleton,
                postFactory: (sp, iw) => 
                    iw.AddCmd(ConsoleKey.Escape, m => sp.GetRequiredService<IHost>().StopAsync().Wait()));
            #endregion

            //AddWorker<BackTestWorker, BackTestOptions>("backtest.json", "1");
            //AddWorker<BackTestWorker, BackTestOptions>(workerId: "2");
            //AddWorker<AutoTradingWorker, AutoTradingWorkerOptions>("autotrading.json");
            
            AddWorker<TickWorker, TickWorkerOptions>("tick.json", "Upbit");
            //AddWorker<TickerWorker, TickerWorkerOptions>(workerId: "Binance");

            RunHost();
        }

    }//class
}


