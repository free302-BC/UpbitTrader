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

        //event와 action 연결 --> event 발생시 action 호출
        readonly OrderedDic<AutoResetEvent, Action> _events;
        readonly object _lock = new object();

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

            //TODO: 
        }

        public override void Dispose()
        {
            _client?.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// event proxy : 쓰레드간 이벤트 중계 
        ///  -> 이벤트 핸들러를 소스에서 수행하지않고 구독 쓰레드에서 수행
        /// 자식클래스의 doWork에서 반드시 먼저 호출해야 함.
        /// </summary>
        protected override Task doWork(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                Thread.CurrentThread.Name = $"{Id}:Event";
                info($"<{Thread.CurrentThread.Name}> Executing...");
                var events = _events.Keys.ToArray();
                while (!stoppingToken.IsCancellationRequested)
                {
                    var index = WaitHandle.WaitAny(events);
                    _events[index]?.Invoke();
                }
            }, stoppingToken);
        }

        protected void registerHotkey(ConsoleKey key, Action handler, ConsoleModifiers mod = 0)
        {
            var @event = new AutoResetEvent(false);
            _cmdProvider.AddSignal(key, @event, mod);

            //event 등록
            lock (_lock)  _events.Add(@event, handler);
        }
        protected void unregisterHotkey(ConsoleKey key, Action handler, ConsoleModifiers mod = 0)
        {
            var @event = _events.Where(p => p.Value == handler).First().Key;
            _cmdProvider.RemoveSignal(key, @event, mod);

            //event 해지
            lock (_lock) _events.Remove(@event);
        }




    }//class

}
