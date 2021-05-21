using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Universe.AppBase
{
    public abstract class WorkerBase<W, S> : IHostedService where W : WorkerBase<W, S> where S : class
    {
        protected readonly ILogger _logger;
        readonly string _name = typeof(W).Name;
        protected readonly IServiceProvider _sp;
        S _set { get; set; }
        public WorkerBase(ILogger<W> logger, IOptionsMonitor<S> set, IServiceProvider sp)
        {
            _logger = logger;
            _set = set.CurrentValue;
            set.OnChange(s => _set = s);
            _sp = sp;
        }

        protected void report(object message)
        {
            var fg = Console.ForegroundColor;
            var bg = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            //Console.BackgroundColor = ConsoleColor.Gray;
            Console.WriteLine($"[{DateTime.Now:yyMMdd.HHmmss}] {message}");
            Console.ForegroundColor = fg;
            //Console.BackgroundColor = bg;
        }

        protected void info(object message) => _logger.LogInformation($"{message}");
        protected void info(object msg1, object msg2) => _logger.LogInformation($"{msg1}{Environment.NewLine}{msg2}");
        protected void info(object msg1, object msg2, object msg3) 
            => _logger.LogInformation($"{msg1}{Environment.NewLine}{msg2}{Environment.NewLine}{msg3}");

        protected void log(object message) => _logger.LogError($"{message}");
        protected void log(object msg1, object msg2) => _logger.LogError($"{msg1}{Environment.NewLine}{msg2}");

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                info($"<{_name}> StartAsync()...");
                work(_set);
                //info($"Worker Exiting StartAsync()...");
            }, cancellationToken)
            .ContinueWith(t => 
            {
                info($"<{_name}> done.");
            });
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            info($"Worker StopAsync()...");
            return Task.CompletedTask;
        }
        protected abstract void work(S set);

    }//class
}
