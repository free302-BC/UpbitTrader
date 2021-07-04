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
    public abstract class TradeWorkerBase<W, S> 
                : WorkerBase<W, S>, IDisposable
        where W : WorkerBase<W, S> 
        where S : TradeWorkerOptions, IClientOptions
    {
        readonly ICommandProvider _cmdProvider;
        protected readonly IClient _client;
        protected readonly JsonSerializerOptions _jsonOptions;
        readonly AutoResetEvent _cmdSignal;

        protected TradeWorkerBase(IServiceProvider sp, string id = "") : base(sp, id)
        {
            _cmdProvider = sp.GetRequiredService<ICommandProvider>();
            _jsonOptions = _jsonOptions.Init();// buildJsonOptions();
            _cmdSignal = new(false);

            var logger = _sp.GetRequiredService<ILogger<IClient>>();
            _client = UvLoader.Create<IClient>(_set.AssemblyFile, _set.ClientFullName,
                _set.GetAccessKey(), _set.GetSecretKey(), logger);
        }

        public override void Dispose()
        {
            _client?.Dispose();
            base.Dispose();
        }

        protected void registerHotkey(ConsoleKey key, CommandAction handler)
        {
            _cmdProvider.AddAction(key, handler);
        }
        protected void unregisterHotkey(ConsoleKey key, CommandAction handler)
        {
            _cmdProvider.RemoveAction(key, handler);
        }

    }//class

}
