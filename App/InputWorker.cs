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
    using EventDic = Dictionary<ConsoleKey, Listener>;
    public delegate void Listener(ConsoleModifiers modifiers);

    public class InputWorker : WorkerBase<InputWorker, WorkerOptions>
    {
        public InputWorker(
            ILogger<InputWorker> logger,
            IServiceProvider sp,
            IOptionsMonitor<WorkerOptions> set)
            : base(logger, sp, set)
        {
            _listeners = new();
            _lock = new();
        }

        readonly EventDic _listeners;
        
        public void AddCmd(ConsoleKey key, Listener cmd, ConsoleModifiers modifiers = 0)
        {
            lock (_lock)
            {
                if (_listeners.ContainsKey(key)) _listeners[key] += cmd;
                else _listeners[key] = cmd;
            }
        }

        public new void info(object? message) => base.info(message);

        object _lock = new object();
        protected override void work()
        {
            while (true)
            {
                var ki = Console.ReadKey(true);
                info($"Invoking {ki.Key} cmd...");

                Listener? cmd = null;
                lock (_lock)
                {
                    if(_listeners.ContainsKey(ki.Key)) cmd = _listeners[ki.Key];
                }
                cmd?.Invoke(ki.Modifiers);
                //Thread.Sleep(100);
            }
        }


    }//class
}
