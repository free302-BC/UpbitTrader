using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Universe.AppBase;
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Model;
using Universe.Coin.Upbit;
using Universe.Coin.Upbit.Model;
using Xunit;
using Xunit.Abstractions;

namespace UnitTester
{
    public class Speed : ProgramBase
    {
        [Fact]
        public void Run()
        {
            AddWorker<TestWorker, WorkerOptionsBase>();
            RunHost();
        }

        public class TestWorker : WorkerBase<TestWorker, WorkerOptionsBase>
        {
            public TestWorker(
                ILogger<TestWorker> logger,
                IServiceProvider sp,
                IOptionsMonitor<WorkerOptionsBase> set, string id = "")
                : base(logger, sp, set, id)
            {
            }

            new void info(object message) => Debug.WriteLine($"{message}");

            protected override void work()
            {
                var logger = _sp.GetRequiredService<ILogger<Client>>();
                var uc = new Client(
                    "E5AEDC2AF2111C0DE0837B20147366A695CAC34FE4137F01E9B2751215561A82CAEBCB44ED343B28",
                    "D4CFF355ED1A7D21C4B14709400E00A1DAA9C267EF360032DAC06F06134625AED9F9F256D230181F", logger);

                var w = Stopwatch.StartNew();
                var models = uc.ApiCandle<CandleMinute>(count: 10000, unit: CandleUnit.M1).ToModels();
                info($"Δt= {w.ElapsedMilliseconds,6}ms: {models[0]}");

                w.Restart();
                IModelCalc.CalcMovingAvg(models, 15);
                info($"Δt= {w.ElapsedMilliseconds,6}ms: {models[0]}");

                _sp.GetRequiredService<IHost>().StopAsync().Wait();
            }
        }


    }//class
}
