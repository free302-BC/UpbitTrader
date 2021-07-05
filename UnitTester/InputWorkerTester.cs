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
    public class InputWorkerTester : ProgramBase
    {
        [Fact] public static void Test()//execute in console 'Tester' app
        {
            AddService<ICommandProvider, InputWorker>(
                start: true,
                postInit: (sp, iw) =>
                {
                    //iw.AddAction(ConsoleKey.Escape, m => sp.GetRequiredService<IHost>().StopAsync().Wait()));
                    iw.OnQuit += () => sp.GetRequiredService<IHost>().StopAsync().Wait();
                    iw.QuitKey = ConsoleKey.Escape;
                });
            AddWorker<TestWorker, TradeWorkerOptions>();
            RunHost();
        }

        public class TestWorker : WorkerBase<TestWorker, TradeWorkerOptions>
        {
            readonly AutoResetEvent _signal;

            public TestWorker(IServiceProvider sp, string id = "") : base(sp, id)
            {
                _signal = new(false);
                var iw = sp.GetRequiredService<ICommandProvider>();
                iw.AddSignal(ConsoleKey.Spacebar, _signal);
            }
            public override void Dispose()
            {
                _signal?.Dispose();
                base.Dispose();
            }

            protected override Task doWork(CancellationToken stoppingToken)
            {
                info($"Entering doWork(): <{Thread.CurrentThread.Name}>:{Thread.CurrentThread.ManagedThreadId}");
                while (!stoppingToken.IsCancellationRequested)
                {
                    _signal.WaitOne();
                    info($"executing cmd: <{Thread.CurrentThread.Name}>:{Thread.CurrentThread.ManagedThreadId}");
                }
                return Task.CompletedTask;
            }
        }


    }//class
}
