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
        /*static */readonly Action<object?> log = Console.WriteLine;
        /*static */readonly List<ServiceAction> _serviceActions;
        /*static */readonly List<ConfigAction> _configActions;
        /*static */readonly List<WorkerAction> _workerActions;

        public ProgramBase()
        {
            _serviceActions = new();
            _configActions = new();
            _workerActions = new();
        }

        public void RunHost()
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
        IHostBuilder createHostBuilder(CancellationTokenSource cts)
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

        static volatile int _counter = 0;
        static List<string> _workerIds = new();
        static List<object?> _posts = new();
        static List<object> _facts = new();
        static Func<IServiceProvider, W> buildFactory<W, S>(
            string workerId, 
            Action<IServiceProvider, W>? postAction,
            int index)
            where W : WorkerBase<W, S> 
            where S : class, IWorkerOptions
        {
            _workerIds.Add(workerId);
            _posts.Add(postAction);

            Func<IServiceProvider, W> func = sp =>
            {
                var w = UvLoader.Create<W>(typeof(W).FullName!, sp, _workerIds[index]);
                (_posts[index] as Action<IServiceProvider, W>)?.Invoke(sp, w);
                return w;
            };
            _facts.Add(func);

            W factory(IServiceProvider sp)
            {
                var w = UvLoader.Create<W>(typeof(W).FullName!, sp, _workerIds[index]);
                (_posts[index] as Action<IServiceProvider, W>)?.Invoke(sp, w);
                return w;
            }
            return func;
        }


        public void AddWorker<W, S>(
            string workerConfigFile = "",
            string workerId = "",
            Action<IServiceProvider, W>? postFactory = null,
            ServiceLifetime lifeTime = ServiceLifetime.Transient)
            where W : WorkerBase<W, S>
            where S : class, IWorkerOptions
        {
            //config            
            if (!string.IsNullOrWhiteSpace(workerConfigFile))
                _configActions.Add(cb => cb.AddJsonFile(workerConfigFile, false, true));

            var factory = buildFactory<W,S>(workerId, postFactory, _counter++);

            _serviceActions.Add((ctx, sc) =>
            {
                _ = lifeTime switch
                {
                    ServiceLifetime.Transient => sc.AddTransient(factory),
                    ServiceLifetime.Singleton => sc.AddSingleton(factory),
                    ServiceLifetime.Scoped => sc.AddScoped(factory),
                    _ => sc.AddTransient(factory)
                };

                //options
                if (string.IsNullOrWhiteSpace(workerId))
                    sc.AddOptions<S>(ctx.Configuration.GetSection(typeof(W).Name));
                else
                    sc.AddOptions<S>(workerId, ctx.Configuration.GetSection($"{typeof(W).Name}:{workerId}"));
            });

            //worker
            _workerActions.Add((sp, token) => sp.GetRequiredService<W>().StartAsync(token));
            //_workerActions.Add((sp, token) => factory(sp).StartAsync(token));
        }

    }//class
}

