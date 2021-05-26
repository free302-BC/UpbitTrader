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
    using ServiceAction = Action<HostBuilderContext, IServiceCollection>;//action ConfigureServices
    using ConfigAction = Action<IConfigurationBuilder>;//action ConfigureAppConfiguration
    using WorkerAction = Action<IServiceProvider, CancellationToken>;//action start worker

    public class ProgramBase
    {
        protected static readonly Action<object?> log = Console.WriteLine;
        static readonly List<ServiceAction> _serviceActions;
        static readonly List<ConfigAction> _configActions;
        static readonly List<WorkerAction> _workerActions;

        static ProgramBase()
        {
            _serviceActions = new List<ServiceAction>();
            _configActions = new List<ConfigAction>();
            _workerActions = new List<WorkerAction>();
        }
        protected static void RunHost()
        {
            try
            {
                //run host
                using CancellationTokenSource cts = new();
                using IHost host = createHostBuilder(cts).Build();
                host.RunAsync(cts.Token)
                    .ContinueWith(
                        t => log($"{nameof(ProgramBase)}.{nameof(RunHost)}(): {t.Exception}"),
                        TaskContinuationOptions.OnlyOnFaulted);

                //run worker
                foreach (var action in _workerActions) action(host.Services, cts.Token);

                host.WaitForShutdown();
            }
            catch (Exception ex)
            {
                log(ex);
            }
        }
        static IHostBuilder createHostBuilder(CancellationTokenSource cts)
        {
            var builder = Host.CreateDefaultBuilder();
            builder.ConfigureAppConfiguration(cb =>
            {
                foreach (var action in _configActions) action(cb);
            });

            builder.ConfigureServices((context, services) =>
            {
                services.AddSingleton(cts);
                services.AddSimpleConsole();
                foreach (var action in _serviceActions) action(context, services);
            });

            return builder;
        }//build

        protected static void AddWorker<W, S>(
            string? workerConfigFile = null,
            IList<string>? workerNames = null,
            Func<IServiceProvider, W>? workerFactory = null)
            where W : WorkerBase<W, S>
            where S : class, IWorkerOptions
        {
            //config            
            if (workerConfigFile != null) _configActions.Add(cb => cb.AddJsonFile(workerConfigFile, false, true));

            //services
            _serviceActions.Add((ctx, sc) =>
            {
                if (workerFactory == null) sc.AddTransient<W>();
                else sc.AddSingleton(workerFactory);

                //options
                if (workerNames == null)
                    sc.AddOptions<S>(ctx.Configuration.GetSection(typeof(W).Name));
                else
                    foreach (var name in workerNames)
                        sc.AddOptions<S>(name, ctx.Configuration.GetSection(name));
            });

            //worker
            _workerActions.Add((sp, token) =>
            {
                ((IHostedService)sp.GetRequiredService<W>()).StartAsync(token);
            });
        }

    }//class
}

