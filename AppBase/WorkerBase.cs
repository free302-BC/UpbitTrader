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
    /// <summary>
    /// IHostedService를 구현하는 베이스 클래스
    /// </summary>
    /// <typeparam name="W"></typeparam>
    /// <typeparam name="S"></typeparam>
    public abstract class WorkerBase<W, S> : IHostedService 
        where W : WorkerBase<W, S> 
        where S : IWorkerOptions
    {
        protected readonly ILogger _logger;
        public string Id { get; set; }
        protected readonly IServiceProvider _sp;
        protected S _set;

        public void Reload(S set) => _set.Reload(set);

        public WorkerBase(IServiceProvider sp, string id = "")
        {
            _sp = sp;
            _logger = _sp.GetRequiredService<ILogger<W>>();
            var set = _sp.GetRequiredService<IOptionsMonitor<S>>();

            if (string.IsNullOrWhiteSpace(id))
            {
                Id = typeof(W).Name;
                _set = set.CurrentValue;
                set.OnChange(s => 
                { 
                    _set.Reload(s);
                    onOptionsUpdate?.Invoke();
                });
            }
            else
            {
                Id = id;
                _set = set.Get(Id);
                set.OnChange((s, n) =>
                {
                    if (n == Id)
                    {
                        _set.Reload(s);
                        onOptionsUpdate?.Invoke();
                    }
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
            .ContinueWith(
                t => log($"{nameof(WorkerBase<W, S>)}.{nameof(StartAsync)}():", t.Exception),
                TaskContinuationOptions.OnlyOnFaulted)
            .ContinueWith(t => info($"<{Id}> done."));
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            info($"Worker StopAsync()...");
            return Task.CompletedTask;
        }

        protected abstract void work();
        protected event Action? onOptionsUpdate;

    }//class
}
