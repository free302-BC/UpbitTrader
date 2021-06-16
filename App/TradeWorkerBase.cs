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
//using Universe.Coin.Upbit;

namespace Universe.Coin.App
{
    public abstract class TradeWorkerBase<W, S> 
                : WorkerBase<W, S>, IDisposable
        where W : WorkerBase<W, S> 
        where S : WorkerOptions, ITradeOptions
    {
        protected readonly InputWorker _inputWorker;
        protected readonly IClient _client;
        protected readonly JsonSerializerOptions _jsonOptions;

        protected TradeWorkerBase(IServiceProvider sp, string id = "") : base(sp, id)
        {
            _inputWorker = sp.GetRequiredService<InputWorker>();
            _jsonOptions = buildJsonOptions();

            var logger = _sp.GetRequiredService<ILogger<Upbit.Client>>();
            _client = UvLoader.Create<IClient>(_set.AssemblyFile, _set.ClientFullName,
                _set.GetAccessKey(), _set.GetSecretKey(), logger);
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
