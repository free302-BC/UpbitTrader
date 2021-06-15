using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Universe.Coin.Upbit;
using Universe.AppBase;
using Microsoft.Extensions.Options;
using System.Threading;
using Microsoft.Extensions.Hosting;

namespace Universe.Coin.App
{
    class Program// : ProgramBase
    {
        static void Main(string[] args)
        {
            var pb = new ProgramBase();

            #region ---- key input listener ----
            pb.AddWorker<InputWorker, WorkerOptions>(
                lifeTime: ServiceLifetime.Singleton,
                postFactory: (sp, iw) => 
                    iw.AddCmd(ConsoleKey.Escape, m => sp.GetRequiredService<IHost>().StopAsync().Wait()));
            #endregion

            pb.AddWorker<TraderWorker, TraderWorkerOptions>();
            pb.AddWorker<TickWorker, TickWorkerOptions>("tickworker.json", "Upbit");
            pb.AddWorker<BackTestWorker, BackTestOptions>("backtest.json", "1");
            pb.AddWorker<BackTestWorker, BackTestOptions>("backtest.json", "2");
            pb.AddWorker<TickWorker, TickWorkerOptions>("tickworker_binance.json", "Binance");
            pb.RunHost();
        }

    }//class
}


