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

namespace Universe.Coin.Upbit.App
{
    class Program : ProgramBase
    {
        static void Main(string[] args)
        {
            //key input listener
            AddWorker<InputWorker, WorkerOptions>(
                lifeTime: ServiceLifetime.Singleton,
                workerFactory: sp =>
                {
                    var opt = sp.GetRequiredService<IOptionsMonitor<WorkerOptions>>();
                    var logger = sp.GetRequiredService<ILogger<InputWorker>>();
                    var iw = new InputWorker(logger, sp, opt);
                    //iw.AddCmd(ConsoleKey.Enter, quit);
                    iw.AddCmd(ConsoleKey.Escape, quit);
                    return iw;

                    void quit(ConsoleModifiers modifiers) => sp.GetRequiredService<IHost>().StopAsync().Wait();
                });

            //AddWorker<BackTestWorker, BackTestOptions>("backtest.json", BackTestWorker.GetIds());
            AddWorker<TraderWorker, TraderOptions>();
            RunHost();
        }

    }//class
}


