using System;
using System.Collections.Generic;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;
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
            //TestWorker.test();
            //Helper.build();
            //return;


            AddWorker<TestWorker, WorkerSetting>();
            RunHost();
        }

    }//class
}


