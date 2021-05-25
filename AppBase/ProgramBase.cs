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

namespace Universe.AppBase
{
    using ACS = Action<HostBuilderContext, IServiceCollection>;//action configure service
    using ACAC = Action<IConfigurationBuilder>;//action ConfigureAppConfiguration

    public class ProgramBase
    {
        protected static readonly Action<object?> log = Console.WriteLine;
        static readonly List<ACS> _acs;
        static readonly List<ACAC> _acac;
        static readonly List<Type> _workers;

        static ProgramBase()
        {
            _acs = new List<ACS>();
            _acac = new List<ACAC>();
            _workers = new List<Type>();
        }
        protected static void RunHost()
        {
            try
            {
                //run host
                using CancellationTokenSource cts = new();
                using IHost host = createHostBuilder().Build();
                host.RunAsync(cts.Token).ContinueWith(t => log(t.Exception), TaskContinuationOptions.OnlyOnFaulted);

                var a = host.Services.GetRequiredService<IConfiguration>();
                //var b = host.Services.GetRequiredService<IConfigurationRoot>();

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
                foreach (var a in _acs) a(context, services);
            });

            //builder.ConfigureAppConfiguration(cb =>
            //{
            //    foreach (var a in _acac) a(cb);
            //});

            return builder;
        }//build

        protected static void AddWorker<W,S>(string? settingsFile = null) where W : WorkerBase<W, S> where S: class
        {
            //config            
            //if (settingsFile != null) _acac.Add(cb => cb.AddJsonFile(settingsFile, false, true));

            //services
            _acs.Add((ctx, sc) =>
            {
                sc.AddTransient<W>();
                //sc.AddOptions<S>(ctx.Configuration, typeof(W).Name);
            });

            //run
            _workers.Add(typeof(W));
        }

    }//class
}
