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

namespace Universe.Coin.Upbit.App
{
    public abstract class WorkerBase<W, S> : IHostedService where W : WorkerBase<W, S> where S : class
    {
        //public static (string ModelName, string ModelFile) GetModelInfo(IConfiguration config)
        //{
        //    var baseKey = $"{typeof(S).Name}:{typeof(W).Name}";
        //    var name = config.Get<string>(baseKey, nameof(ModelSetting.Model), nameof(ModelSetting.Model.ModelName));//.GetValue<string>(nameKey);
        //    var dir = config.Get<string>(baseKey, nameof(ModelSetting.DataDir));
        //    var file = Path.Combine(Environment.CurrentDirectory, dir, $"{name}.zip");
        //    return (name, file);
        //}

        readonly ILogger _logger;
        S _set;

        public WorkerBase(ILogger<W> logger, IOptionsMonitor<S> set)
        {
            _logger = logger;
            _set = set.CurrentValue;
            set.OnChange(s => _set = s);
        }

        protected void info(object message) => _logger.LogInformation($"{message}");
        protected void log(object message) => _logger.LogError($"{message}");

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                info($"Worker Entering StartAsync()...");
                work(_set);
                info($"Worker Exiting StartAsync()...");
            }, cancellationToken);
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            info($"Worker StopAsync()...");
            return Task.CompletedTask;
        }
        protected abstract void work(S set);

    }//class
}
