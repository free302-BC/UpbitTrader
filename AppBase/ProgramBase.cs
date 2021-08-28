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
using System.IO;
using Universe.Logging;

namespace Universe.AppBase
{
    using ServiceAction = Action<HostBuilderContext, IServiceCollection>;//action in ConfigureServices
    using ConfigAction = Action<IConfigurationBuilder>;//action in ConfigureAppConfiguration
    using WorkerAction = Action<IServiceProvider, CancellationToken>;//action start worker

    /// <summary>
    /// 
    /// </summary>
    public class ProgramBase
    {
        static readonly Action<object?> log = Console.WriteLine;
        static readonly List<ServiceAction> _serviceActions;
        static readonly List<ConfigAction> _configActions;

        static ProgramBase()
        {
            _serviceActions = new();
            _configActions = new();
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
            builder.UseUvLog();

            builder.ConfigureAppConfiguration(cb =>
            {
                foreach (var action in _configActions) action(cb);
            });

            builder.ConfigureServices((context, services) =>
            {
                //services.AddSingleton(cts);//TODO: 필요?
                services.AddSimpleConsole();
                foreach (var action in _serviceActions) action(context, services);
            });

            return builder;
        }//build


        static Queue<string> _idQ = new();//등록된 id를 저장

        /// <summary>
        /// BackgroundWorker를 옵션 및 설정파일과 함께 추가
        /// </summary>
        /// <typeparam name="W">service type</typeparam>
        /// <param name="workerId">같은 타입의 워커 인스턴스를 구별할 아이디</param>
        /// <param name="postInit">instance 생성직후 수행할 액션</param>
        static protected void AddWorker<W>(
            string workerId = "",
            Action<IServiceProvider, W>? postInit = null)
            where W : BackgroundService
        {
            AddWorker<W, W>(workerId, postInit);
        }

        /// <summary>
        /// 인터페이스 I를 구현하는 background worker W를 등록
        /// </summary>
        /// <typeparam name="I">interface</typeparam>
        /// <typeparam name="W">implimenting class</typeparam>
        /// <param name="postInit"></param>
        static protected void AddWorker<I, W>(
            string workerId = "",
            Action<IServiceProvider, W>? postInit = null)
            where I : class, IHostedService
            where W : BackgroundService, I
        {
            _serviceActions.Add((ctx, sc) =>
            {
                _idQ.Enqueue(workerId);

                W factory(IServiceProvider sp)
                {
                    var w = ActivatorUtilities.CreateInstance<W>(sp, _idQ.Dequeue());
                    postInit?.Invoke(sp, w);
                    return w;
                }
                sc.AddSingleton<I>(factory);
                sc.AddHostedService<I>(sp => sp.GetRequiredService<I>());
            });
        }

        /// <summary>
        /// 인터페이스 I를 구현하는 service W를 등록
        /// </summary>
        /// <typeparam name="I">interface</typeparam>
        /// <typeparam name="W">class implimenting I</typeparam>
        /// <param name="instance"></param>
        /// <param name="factory"></param>
        /// <param name="life"></param>
        static protected void AddService<I, W>(
            W? instance = default,
            Func<IServiceProvider, W>? factory = null,
            ServiceLifetime life = ServiceLifetime.Singleton)
            where I : class
            where W : class, I
        {
            _serviceActions.Add((ctx, sc) =>
            {
                if (instance != null) sc.AddSingleton(instance);
                else if (factory != null)
                {
                    _ = life switch
                    {
                        ServiceLifetime.Singleton => sc.AddScoped(factory),
                        ServiceLifetime.Scoped => sc.AddScoped(factory),
                        ServiceLifetime.Transient => sc.AddTransient(factory),
                        _ => throw new ArgumentException("Inconsistent parameters")
                    };
                }
            });
        }


        /// <summary>
        /// W에 대한 옵션 O와 설정파일을 등록
        /// </summary>
        /// <typeparam name="W"></typeparam>
        /// <typeparam name="O"></typeparam>
        /// <param name="workerConfigFile">설정 json 파일</param>
        static protected void AddWorkerOption<W, O>(
            string workerConfigFile = "")
            where W : WorkerBase<W, O>
            where O : class, IWorkerOptions
        {
            //config            
            if (!string.IsNullOrWhiteSpace(workerConfigFile))
                _configActions.Add(cb => cb.AddJsonFile(workerConfigFile, false, true));

            //options
            _serviceActions.Add((ctx, sc)
                => sc.AddOptions<O>(ctx.Configuration.GetSection(typeof(W).Name)));
        }

    }//class
}

