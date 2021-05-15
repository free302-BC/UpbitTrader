using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Universe.Coin.Upbit.App
{
    public class ProgramBase
    {
        protected static readonly Action<object?> log = Console.WriteLine;
        static List<Action<ConfigTuple>> _configs;
        static List<Type> _workers;

        static ProgramBase()
        {
            _configs = new List<Action<ConfigTuple>>();
            _workers = new List<Type>();
        }
        protected static void RunHost()
        {
            try
            {
                //run host
                using CancellationTokenSource cts = new CancellationTokenSource();
                using IHost host = createHostBuilder().Build();
                host.RunAsync(cts.Token).ContinueWith(t => log(t.Exception), TaskContinuationOptions.OnlyOnFaulted);

                //run worker
                foreach (var m in _workers) ((IHostedService)host.Services.GetRequiredService(m)).StartAsync(cts.Token);

                host.WaitForShutdown();
            }
            catch (Exception ex)
            {
                log(ex);
            }
        }
        static IHostBuilder createHostBuilder()
        {
            var builder = Host.CreateDefaultBuilder();
            builder.ConfigureServices((context, services) =>
            {
                services.AddSimpleConsole();
                foreach (var config in _configs) config.Invoke((context.Configuration, services));
            });
            return builder;
        }//build

        protected static void AddWorker<W,S>() where W : WorkerBase<W, S> where S: class
        {
            //config
            _configs.Add(ct =>
            { 
                ct.Services.AddTransient<W>();
                ct.AddSetting<S>(typeof(W).Name);
            });

            //run
            _workers.Add(typeof(W));
        }

    }//class
}
