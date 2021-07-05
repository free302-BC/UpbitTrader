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
    using ActionDic = Dictionary<ConsoleKey, CommandAction>;
    using SignalDic = Dictionary<ConsoleKey, List<EventWaitHandle>>;

    public class InputWorker : WorkerBase<InputWorker, TradeWorkerOptions>, ICommandProvider
    {
        readonly ActionDic _actions;
        readonly SignalDic _signals;
        readonly object _lock;

        public InputWorker(IServiceProvider sp) : base(sp, "")
        {
            _actions = new();
            _signals = new();
            _lock = new();
        }

        //특별 종료 키 
        public ConsoleKey QuitKey { get; set; }
        public event Action? OnQuit;

        /// <summary>
        /// InputWorker의 doWork()와 같은 쓰레드에서 수행되는 코드 등록
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cmd"></param>
        public void AddAction(ConsoleKey key, CommandAction cmd)
        {
            lock (_lock)
            {
                if (_actions.ContainsKey(key)) _actions[key] += cmd;
                else _actions[key] = cmd;
            }
        }
        public void RemoveAction(ConsoleKey key, CommandAction cmd)
        {
            lock (_lock)
            {
                if (_actions.ContainsKey(key))
                {
                    var result = _actions[key] - cmd;
                    if (result is null) _actions.Remove(key);
                    else _actions[key] = result;
                }
            }
        }

        /// <summary>
        /// 이벤트시 신호를 수신하는 핸들 등록
        /// </summary>
        /// <param name="key"></param>
        /// <param name="signal"></param>
        public void AddSignal(ConsoleKey key, EventWaitHandle signal)
        {
            lock (_lock)
            {
                if (_signals.ContainsKey(key)) _signals[key].Add(signal);
                else _signals[key] = new() { signal };
            }
        }
        public void RemoveSignal(ConsoleKey key, EventWaitHandle signal)
        {
            lock (_lock)
            {
                if (_signals.ContainsKey(key))
                {
                    var result = _signals[key].Remove(signal);
                    if (result && _signals[key].Count == 0) _signals.Remove(key);
                }
            }
        }

        protected override Task doWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var ki = Console.ReadKey(true);
                //info($"Invoking {ki.Key} cmd...");

                if (ki.Key == QuitKey) OnQuit?.Invoke();

                CommandAction? cmd = null;
                List<EventWaitHandle>? list = null;
                lock (_lock)
                {
                    if (_actions.ContainsKey(ki.Key)) cmd = _actions[ki.Key];
                    if (_signals.ContainsKey(ki.Key)) list = _signals[ki.Key];
                }

                try
                {
                    cmd?.Invoke(ki.Modifiers);
                    if (list != null) foreach (var h in list) h.Set();
                }
                catch (Exception ex)
                {
                    logEx(ki, ex);
                }
            }
            return Task.CompletedTask;

            void logEx(ConsoleKeyInfo ki, Exception? ex)
            {
                var key = ki.Modifiers == 0 ? ki.Key.ToString() : $"{ki.Modifiers}, {ki.Key}";
                log($"Error executing <{key}>", ex);
            }
        }

    }//class
}
