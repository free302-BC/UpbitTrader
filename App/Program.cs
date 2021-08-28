using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Universe.AppBase;
using Microsoft.Extensions.Options;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Universe.Logging;


namespace Universe.Coin.App
{
    class Program : ProgramBase
    {
        static void Main(string[] args)
        {

            #region ---- console key listener ----

            AddWorker<IInputProvider, InputWorker>(
                postInit: (sp, iw) =>
                {
                    var w = iw as InputWorker ?? throw new InvalidCastException();
                    //iw.AddAction(ConsoleKey.Escape, m => sp.GetRequiredService<IHost>().StopAsync().Wait()));
                    w.OnQuit += () => sp.GetRequiredService<IHost>().StopAsync().Wait();
                    w.QuitKey = ConsoleKey.Escape;
                });

            #endregion

            //AddWorker<BackTestWorker>("1");
            //AddWorkerOption<BackTestWorker, BackTestOptions>("backtest.json");
            //AddWorker<BackTestWorker>(workerId: "2");
            //AddWorker<AutoTradingWorker>();
            //AddWorkerOption<AutoTradingWorker, AutoTradingWorkerOptions>("autotrading.json");

            AddWorker<TickWorker>("Upbit");
            AddWorkerOption<TickWorker, TickWorkerOptions>("tick.json");
            AddWorker<TickWorker>(workerId: "Binance");

            RunHost();
        }

    }//class
}


