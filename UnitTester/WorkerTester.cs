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
using Universe.Coin.TradeLogic.Calc;
using Universe.Coin.TradeLogic.Model;
using Universe.Coin.Upbit;
using Universe.Coin.App;
using Universe.Coin.Upbit.Model;
using Xunit;
using Xunit.Abstractions;

namespace UnitTester
{
    public class WorkerTester : ProgramBase
    {
        const string _accessKey = "E5AEDC2AF2111C0DE0837B20147366A695CAC34FE4137F01E9B2751215561A82CAEBCB44ED343B28";
        const string _secretKey = "D4CFF355ED1A7D21C4B14709400E00A1DAA9C267EF360032DAC06F06134625AED9F9F256D230181F";
        readonly Client _client;
        readonly ILogger _logger;

        public WorkerTester()
        {
            _logger = null!;

            IClientOptions opt = new TradeOptions()
            {
                AccessKey = _accessKey,
                SecretKey = _secretKey
            };

            _client = new Client(opt.GetAccessKey(), opt.GetSecretKey(), _logger);
        }

        [Fact]
        public void Run()
        {
            AddWorker<TestWorker>();
            RunHost();
        }

        [Fact]
        void apiParamTest()
        {
            var param = new ApiParam();
            var result = _client.ApiTest(param);
            Debug.WriteLine(result);
        }

        public class TradeOptions : IClientOptions
        {
            public string AccessKey { get; set; }
            public string SecretKey { get; set; }
            public string AssemblyFile { get; set; }
            public string ClientFullName { get; set; }
        }


        public class TestWorker : WorkerBase<TestWorker, TradeWorkerOptions>
        {
            readonly Client _client;
            public TestWorker(IServiceProvider sp, string id = "") : base(sp, id)
            {
                IClientOptions opt = new TradeOptions()
                {
                    AccessKey = _accessKey,
                    SecretKey = _secretKey
                };

                _client = new Client(opt.GetAccessKey(), opt.GetSecretKey(), _logger);
            }

            new void info(object message) => Debug.WriteLine($"{message}");
            new void info(object message1, object message2) => Debug.WriteLine($"{message1}\n{message2}");

            protected override Task doWork(CancellationToken stoppingToken)
            {
                MovingAvg_Speed_Test();
                _sp.GetRequiredService<IHost>().StopAsync().Wait();
                return Task.CompletedTask;
            }

            void MovingAvg_Speed_Test()
            {
                var w = Stopwatch.StartNew();
                var models = _client.ApiCandle<CandleMinute>(count: 100000, unit: CandleUnit.M1).ToModels();
                info($"Load Models: Δt= {w.ElapsedMilliseconds}ms", $"{models[0]}");

                var param = new CalcParam()
                {
                    //ApplyMovingAvg = true,
                    WindowSize = 15,
                    WindowFunction = WindowFunction.Identical
                };

                w.Restart();
                param.WindowFunction = WindowFunction.None;
                ICalc.CalcMovingAvg(models, param);
                info($"{param.WindowFunction,20}:Δt= {w.ElapsedMilliseconds}ms", $"{models[0]}");

                w.Restart();
                param.WindowFunction = WindowFunction.Identical;
                ICalc.CalcMovingAvg(models, param);
                info($"{param.WindowFunction,20}:Δt= {w.ElapsedMilliseconds}ms", $"{models[0]}");

                param.WindowFunction = WindowFunction.Linear;
                w.Restart();
                ICalc.CalcMovingAvg(models, param);
                info($"{param.WindowFunction,20}Δt= {w.ElapsedMilliseconds}ms", $"{models[0]}");

                param.WindowFunction = WindowFunction.Gaussian;
                w.Restart();
                ICalc.CalcMovingAvg(models, param);
                info($"{param.WindowFunction,20}:Δt= {w.ElapsedMilliseconds}ms", $"{models[0]}");
            }

        }


    }//class
}
