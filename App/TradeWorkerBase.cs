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

namespace Universe.Coin.Upbit.App
{
    public abstract class TradeWorkerBase<W, S> 
                : WorkerBase<W, S>, IDisposable
        where W : WorkerBase<W, S> 
        where S : WorkerOptions, ITradeOptions
    {
        protected readonly InputWorker _inputWorker;
        protected readonly Client _client;
        protected readonly JsonSerializerOptions _jsonOptions;

        protected TradeWorkerBase(
            ILogger<W> logger, 
            IServiceProvider sp, 
            IOptionsMonitor<S> set,
            InputWorker inputWorker,
            string id = "") : base(logger, sp, set, id)
        {
            _inputWorker = inputWorker;
            _jsonOptions = buildJsonOptions();
            _client = new Client(_set.GetAccessKey(), _set.GetSecretKey(), _sp.GetRequiredService<ILogger<Client>>());
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

        protected void registerHotkey(ConsoleKey key, InputWorker.Listener handler)
        {
            _inputWorker.AddCmd(key, handler);
        }

        static JsonSerializerOptions buildJsonOptions()
        {
            var opt = new JsonSerializerOptions();
            opt.IncludeFields = true;
            opt.WriteIndented = true;
            opt.PropertyNameCaseInsensitive = false;
            opt.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            opt.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.HangulSyllables);
            return opt;
        }

    }//class

}
