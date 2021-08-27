using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Universe.Logging;

namespace Universe.AppBase
{
    /// <summary>
    /// IHostedService를 구현하는 베이스 클래스
    /// </summary>
    /// <typeparam name="W"></typeparam>
    /// <typeparam name="S"></typeparam>
    public abstract class WorkerBase<W, S> : BackgroundService//IHostedService
        where W : WorkerBase<W, S>
        where S : IWorkerOptions
    {
        protected readonly IServiceProvider _sp;
        protected readonly ILogger _logger;
        protected readonly IConfiguration _config;
        public string Id { get; set; }
        protected S _set;
        protected event Action? onOptionsUpdate;

        public WorkerBase(IServiceProvider sp, string id = "")
        {
            _sp = sp;
            _logger = _sp.GetRequiredService<ILogger<W>>();
            _config = _sp.GetRequiredService<IConfiguration>();
            var set = _sp.GetRequiredService<IOptionsMonitor<S>>();

            Id = string.IsNullOrWhiteSpace(id) ? typeof(W).Name : $"{typeof(W).Name}:{id}";
            _set = set.CurrentValue;
            _config.GetSection(Id).Bind(_set);
            set.OnChange(s =>
            {
                _config.GetSection(Id).Bind(_set);
                onOptionsUpdate?.Invoke();
            });
        }

        protected abstract Task doWork(CancellationToken stoppingToken);

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                Thread.CurrentThread.Name = $"{Id}";
                info($"<{Id}> ExecuteAsync()...");

                doWork(stoppingToken);

            }, stoppingToken)

            .ContinueWith(
                t => log($"{Id}.{nameof(ExecuteAsync)}():", t.Exception),
                TaskContinuationOptions.OnlyOnFaulted)

            .ContinueWith(t => info($"<{Id}> done."));
        }


        #region ---- Log & Report ----

        protected void report(object message, int color = 0)
        {
            var fg = Console.ForegroundColor;
            if (color != 0) Console.ForegroundColor = color > 0 ? ConsoleColor.DarkYellow : ConsoleColor.DarkGreen;
            else Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"[{DateTime.Now:yyMMdd.HHmmss.fff}] {message}");
            Console.ForegroundColor = fg;
        }

        protected void info(object? message) => _logger.Info(message);
        protected void info(object? msg1, object? msg2) => _logger.Info(msg1, msg2);
        protected void log(object? msg1, Exception? ex = default) => _logger.Error(msg1, ex);
        protected void log(object? msg1, object? msg2, Exception? ex = default) => _logger.Error(msg1, msg2, ex);

        #endregion

    }//class
}
