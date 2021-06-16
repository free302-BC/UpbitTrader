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
            AddWorker<InputWorker, WorkerOptions>(
                lifeTime: ServiceLifetime.Singleton,
                postFactory: (sp, iw) => 
                    iw.AddCmd(ConsoleKey.Escape, m => sp.GetRequiredService<IHost>().StopAsync().Wait()));
            #endregion

            //AddWorker<BackTestWorker, BackTestOptions>("backtest.json", "1");
            //AddWorker<BackTestWorker, BackTestOptions>(workerId: "2");
            //AddWorker<TraderWorker, TraderWorkerOptions>();
            AddWorker<TickWorker, TickWorkerOptions>("tickworker.json", "Upbit");
            //AddWorker<TickWorker, TickWorkerOptions>("tickworker_binance.json", "Binance");
            RunHost();
        }

    }//class
}


