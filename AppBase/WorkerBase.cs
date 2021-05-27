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

namespace Universe.AppBase
{
    public abstract class WorkerBase<W, S> : IHostedService where W : WorkerBase<W, S> where S : IWorkerOptions
    {
        protected readonly ILogger _logger;
        public string Id { get; set; }
        protected readonly IServiceProvider _sp;
        protected S _set;

        public void Reload(S set) => _set.Reload(set);

        public WorkerBase(ILogger<W> logger, IServiceProvider sp, IOptionsMonitor<S> set, string id = "")
        {
            _logger = logger;
            _sp = sp;

            if (string.IsNullOrWhiteSpace(id))
            {
                Id = typeof(W).Name;
                _set = set.CurrentValue;
                set.OnChange(s => _set.Reload(s));
            }
            else
            {
                Id = id;
                _set = set.Get(Id);
                set.OnChange((s, n) =>
                {
                    if (n == Id) _set.Reload(s);
                });
            }
        }

        protected void report(object message, int color = 0)
        {
            var fg = Console.ForegroundColor;
            if (color != 0) Console.ForegroundColor = color > 0 ? ConsoleColor.DarkYellow : ConsoleColor.DarkGreen;
            else Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"[{DateTime.Now:yyMMdd.HHmmss.fff}] {message}");
            Console.ForegroundColor = fg;
        }

        protected void info(object? message) => _logger.LogInformation($"{message}");
        protected void info(object? msg1, object? msg2) => _logger.LogInformation($"{msg1}{Environment.NewLine}{msg2}");
        protected void info(object? msg1, object? msg2, object? msg3)
            => _logger.LogInformation($"{msg1}{Environment.NewLine}{msg2}{Environment.NewLine}{msg3}");

        protected void log(object? message) => _logger.LogError($"{message}");
        protected void log(object? msg1, object? msg2) => _logger.LogError($"{msg1}{Environment.NewLine}{msg2}");

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                info($"<{Id}> StartAsync()...");
                work();
            }, cancellationToken)
            .ContinueWith(t => info($"<{Id}> done."))
            .ContinueWith(
                t => log($"{nameof(WorkerBase<W, S>)}.{nameof(StartAsync)}():", t.Exception), 
                TaskContinuationOptions.OnlyOnFaulted);
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            info($"Worker StopAsync()...");
            return Task.CompletedTask;
        }

        protected abstract void work();

    }//class
}
