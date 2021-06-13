using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic
{
    public interface ITradeClientBase : IDisposable
    {
        JsonSerializerOptions JsonOptions { get; }
        void ConnectWs(IWsRequest request);

        event Action<string>? OnWsReceived;
        void Pause(bool doPause);

        T[] InvokeApi<T>(ApiId apiId, string postPath = "") where T : IApiModel, new();

        static ITradeClientBase()
        {
            _jsonOption = new JsonSerializerOptions
            {
                IncludeFields = true,
                WriteIndented = true,
                PropertyNameCaseInsensitive = false
            };
            _jsonOption.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            _jsonOption.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.HangulSyllables);
        }

        protected static readonly JsonSerializerOptions _jsonOption;        
        
    }

}