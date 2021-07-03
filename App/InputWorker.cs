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
using System.IO;
using System.Collections.Specialized;
using Universe.Utility;

namespace Universe.Coin.App
{
    using EventDic = Dictionary<ConsoleKey, CommandListener>;

    public class InputWorker : WorkerBase<InputWorker, TradeWorkerOptions>, ICommandProvider
    {

        public InputWorker(IServiceProvider sp) : base(sp, "")
        {
            _listeners = new();
            _lock = new();
        }

        readonly EventDic _listeners;
        readonly object _lock;

        public void AddCmd(ConsoleKey key, CommandListener cmd)
        {
            lock (_lock)
            {
                if (_listeners.ContainsKey(key)) _listeners[key] += cmd;
                else _listeners[key] = cmd;
            }
        }
        public void RemoveCmd(ConsoleKey key, CommandListener cmd)
        {
            lock (_lock)
            {
                if (_listeners.ContainsKey(key))
                {
                    var result = _listeners[key] - cmd;
                    if (result is null) _listeners.Remove(key);
                    else _listeners[key] = result;
                }
            }
        }

        protected override Task doWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var ki = Console.ReadKey(true);
                //info($"Invoking {ki.Key} cmd...");

                CommandListener? cmd = null;
                lock (_lock)
                {
                    if (_listeners.ContainsKey(ki.Key)) cmd = _listeners[ki.Key];
                }
                cmd?.Invoke(ki.Modifiers);
                //Thread.Sleep(100);
            }
            return Task.CompletedTask;
        }
        //protected override void workDone(){}


    }//class
}
