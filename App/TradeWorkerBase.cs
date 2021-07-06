using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Universe.AppBase;
using Universe.Coin.TradeLogic;
using Universe.Utility;
using System.Threading;
//using Universe.Coin.Upbit;

namespace Universe.Coin.App
{
    /// <summary>
    /// Universe.Coin.TradeLogic.IClient를 이용하는 worker
    /// </summary>
    /// <typeparam name="W"></typeparam>
    /// <typeparam name="S"></typeparam>
    public abstract class TradeWorkerBase<W, S> : WorkerBase<W, S>, IDisposable
        where W : WorkerBase<W, S>
        where S : TradeWorkerOptions, IClientOptions
    {
        readonly IInputProvider _cmdProvider;
        protected readonly IClient _client;
        protected readonly JsonSerializerOptions _jsonOptions;
        readonly OrderedDic<AutoResetEvent, Action> _events;

        protected TradeWorkerBase(IServiceProvider sp, string id = "") : base(sp, id)
        {
            _events = new();
            _cmdProvider = sp.GetRequiredService<IInputProvider>();

            //TODO: 필요?
            _jsonOptions = new JsonSerializerOptions().Init();

            //init client
            var logger = _sp.GetRequiredService<ILogger<IClient>>();
            _client = UvLoader.Create<IClient>(_set.AssemblyFile, _set.ClientFullName,
                _set.GetAccessKey(), _set.GetSecretKey(), logger);
        }

        public override void Dispose()
        {
            _client?.Dispose();
            base.Dispose();
        }

        protected override Task doWork(CancellationToken stoppingToken)
        {
            var events = _events.Keys.ToArray();
            while (!stoppingToken.IsCancellationRequested)
            {
                var index = WaitHandle.WaitAny(events);
                _events[index]?.Invoke();
            }
            return Task.CompletedTask;
        }

        protected void registerHotkey(ConsoleKey key, Action handler, ConsoleModifiers mod = 0)
        {
            var @event = new AutoResetEvent(false);
            _cmdProvider.AddSignal(key, @event, mod);
            _events.Add(@event, handler);
        }
        protected void unregisterHotkey(ConsoleKey key, Action handler, ConsoleModifiers mod = 0)
        {
            var @event = _events.Where(p => p.Value == handler).FirstOrDefault().Key;
            _cmdProvider.RemoveSignal(key, @event, mod);
            _events.Remove(@event);
        }




    }//class

}
