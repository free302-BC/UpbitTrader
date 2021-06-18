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
    using EventDic = Dictionary<ConsoleKey, InputWorker.Listener>;

    public class InputWorker : WorkerBase<InputWorker, WorkerOptions>
    {
        public delegate void Listener(ConsoleModifiers modifiers);

        public InputWorker(IServiceProvider sp, string id = "") : base(sp, id)
        {
            _listeners = new();
            _lock = new();
        }

        readonly EventDic _listeners;
        
        public void AddCmd(ConsoleKey key, Listener cmd)
        {
            lock (_lock)
            {
                if (_listeners.ContainsKey(key)) _listeners[key] += cmd;
                else _listeners[key] = cmd;
            }
        }
        public void RemoveCmd(ConsoleKey key, Listener cmd)
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

        object _lock = new object();
        protected override void doWork()
        {
            while (true)
            {
                var ki = Console.ReadKey(true);
                //info($"Invoking {ki.Key} cmd...");

                Listener? cmd = null;
                lock (_lock)
                {
                    if(_listeners.ContainsKey(ki.Key)) cmd = _listeners[ki.Key];
                }
                cmd?.Invoke(ki.Modifiers);
                //Thread.Sleep(100);
            }
        }
        //protected override void workDone(){}


    }//class
}
