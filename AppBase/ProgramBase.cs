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
using Microsoft.Extensions.Options;
using Universe.Utility;

namespace Universe.AppBase
{
    using ServiceAction = Action<HostBuilderContext, IServiceCollection>;//action ConfigureServices
    using ConfigAction = Action<IConfigurationBuilder>;//action ConfigureAppConfiguration
    using WorkerAction = Action<IServiceProvider, CancellationToken>;//action start worker

    public class ProgramBase
    {
        static readonly Action<object?> log = Console.WriteLine;
        static readonly List<ServiceAction> _serviceActions;
        static readonly List<ConfigAction> _configActions;
        static readonly List<WorkerAction> _workerActions;

        static ProgramBase()
        {
            _serviceActions = new();
            _configActions = new();
            _workerActions = new();
        }

        public static void RunHost()
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


        static Queue<string> _idQ = new();//등록된 id를 저장

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="W">service type</typeparam>
        /// <typeparam name="S">option type</typeparam>
        /// <param name="workerConfigFile"></param>
        /// <param name="workerId"></param>
        /// <param name="postFactory">instance 생성후 수행할 액션</param>
        /// <param name="lifeTime"></param>
        static protected void AddWorker<W, S>(
            string workerConfigFile = "",
            string workerId = "",
            Action<IServiceProvider, W>? postFactory = null,
            ServiceLifetime lifeTime = ServiceLifetime.Singleton)
            where W : WorkerBase<W, S>
            where S : class, IWorkerOptions
        {
            //config            
            if (!string.IsNullOrWhiteSpace(workerConfigFile))
                _configActions.Add(cb => cb.AddJsonFile(workerConfigFile, false, true));

            _serviceActions.Add((ctx, sc) =>
            {
                _idQ.Enqueue(workerId);
                W factory(IServiceProvider sp)
                {
                    var w = ActivatorUtilities.CreateInstance<W>(sp, _idQ.Dequeue());
                    postFactory?.Invoke(sp, w);
                    return w;
                }
                _ = lifeTime switch
                {
                    ServiceLifetime.Singleton => sc.AddSingleton(factory),
                    ServiceLifetime.Scoped => sc.AddScoped(factory),
                    _ => sc.AddTransient(factory)
                };

                //options
                sc.AddOptions<S>(ctx.Configuration.GetSection(typeof(W).Name));
            });

            //worker
            _workerActions.Add((sp, token) => ((IHostedService)sp.GetRequiredService<W>()).StartAsync(token));
        }

        static protected void AddService<S, W>(bool start, Action<IServiceProvider, W>? postFactory = null)
            where S : class
            where W : class, S
        {
            _serviceActions.Add((ctx, sc) =>
            {
                W factory(IServiceProvider sp)
                {
                    var w = ActivatorUtilities.CreateInstance<W>(sp);
                    postFactory?.Invoke(sp, w);
                    return w;
                }
                sc.AddSingleton<S>(factory);
            });

            //worker
            if (start)
                _workerActions.Add((sp, token) => ((IHostedService)sp.GetRequiredService<S>()).StartAsync(token));
        }

    }//class
}

