using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Universe.AppBase;
using System.Text.Json;
using Universe.Coin.Upbit.Model;
using System.IO;
using System.Collections.Specialized;
using Universe.Utility;

namespace Universe.Coin.Upbit.App
{
    using CMD = Action;
    public class InputWorker : WorkerBase<InputWorker, WorkerOptionsBase>
    {
        public InputWorker(
            ILogger<InputWorker> logger,
            IServiceProvider sp,
            IOptionsMonitor<WorkerOptionsBase> set)
            : base(logger, sp, set)
        {
            _listeners = new();
            _lock = new();
        }

        Dictionary<ConsoleKey, CMD> _listeners;
        public void AddCmd(ConsoleKey key, CMD cmd)
        {
            lock (_lock) _listeners.Add(key, cmd);
        }
        public new void info(object? message) => base.info(message);

        object _lock = new object();
        protected override void work()
        {
            while (true)
            {
                var ki = Console.ReadKey(true);
                info($"Invoking {ki.Key} cmd...");

                CMD? cmd = null;
                lock (_lock)
                {
                    if(_listeners.ContainsKey(ki.Key)) cmd = _listeners[ki.Key];
                }
                cmd?.Invoke();

                //Thread.Sleep(100);
            }
        }


    }//class
}
