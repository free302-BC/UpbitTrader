using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Universe.Coin.Upbit;
using Universe.AppBase;

namespace Universe.Coin.Upbit.App
{
    class Program : ProgramBase
    {
        static void Main(string[] args)
        {
            //Helper.buildCoinNameJson();

            AddWorker<BackTestWorker, BackTestOptions>("backtest.json", BackTestWorker.GetIds());
            //AddWorker<TraderWorker, TraderOptions>();
            RunHost();
        }

    }//class
}


