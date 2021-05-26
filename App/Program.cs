﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Universe.Coin.Upbit;
using Universe.AppBase;
using Microsoft.Extensions.Options;
using System.Threading;

namespace Universe.Coin.Upbit.App
{
    class Program : ProgramBase
    {
        static void Main(string[] args)
        {
            //Helper.buildCoinNameJson();
            AddWorker<InputWorker, WorkerOptionsBase>(workerFactory: sp =>
            {
                var logger = sp.GetRequiredService<ILogger<InputWorker>>();
                var opt = sp.GetRequiredService<IOptionsMonitor<WorkerOptionsBase>>();
                var iw = new InputWorker(logger, sp, opt);
                iw.AddCmd(ConsoleKey.F1, () => log("test cmd <1> executing..."));                
                return iw;
            });

            AddWorker<BackTestWorker, BackTestOptions>("backtest.json", BackTestWorker.GetIds());
            //AddWorker<TraderWorker, TraderOptions>();
            RunHost();
        }

    }//class
}


