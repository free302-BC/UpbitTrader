using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Universe.AppBase;
using Microsoft.Extensions.Options;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Universe.AppBase.Logging;

namespace Universe.Coin.App
{
    class Program : ProgramBase
    {
        static void Main(string[] args)
        {

            #region ---- console key listener ----

            AddService<IInputProvider, InputWorker>(
                start: true,
                postInit: (sp, iw) =>
                {
                    //iw.AddAction(ConsoleKey.Escape, m => sp.GetRequiredService<IHost>().StopAsync().Wait()));
                    iw.OnQuit += () => sp.GetRequiredService<IHost>().StopAsync().Wait();
                    iw.QuitKey = ConsoleKey.Escape;
                });

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


