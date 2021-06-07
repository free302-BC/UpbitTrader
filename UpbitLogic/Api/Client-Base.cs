using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Model;
using Universe.Coin.Upbit.Model;
using Universe.Utility;

namespace Universe.Coin.Upbit
{
    using JS = System.Text.Json.JsonSerializer;

    /// <summary>
    /// API client base
    /// </summary>
    public abstract class ClientBase : IDisposable
    {
        const string _wssUri = "wss://api.upbit.com/websocket/v1";
        const string _market = "KRW-BTC";
        readonly KeyPair _key;
        readonly ILogger _logger;
        readonly WebClient _wc;
        readonly ClientWebSocket _ws;
        readonly CancellationTokenSource _cts;

        public ClientBase(string accessKeyEnc, string secretKeyEnc, ILogger logger)
        {
            _logger = logger;
            _key = (accessKeyEnc.Decode(), secretKeyEnc.Decode());
            _wc = new();
            _wc.SetAuthToken(_key);
            _wc.SetAcceptance();

            _ws = new();
            _ws.Options.KeepAliveInterval = new TimeSpan(0, 1, 30);
            _cts = new();
            _evPausing = new(false);
        }
        public void Dispose()
        {
            _wc?.Dispose();
            _ws?.Dispose();
            _cts?.Dispose();
            _evPausing?.Dispose();
        }


        #region ---- WebSocket ----

        /// <summary>
        /// 
        /// </summary>
        public event Action<string>? OnReceived;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doPause"></param>
        public void Pause(bool doPause)
        {
            if (doPause) _evPausing.Reset();
            else _evPausing.Set();
        }
        readonly ManualResetEvent _evPausing;

        /// <summary>
        /// 
        /// </summary>
        public void ConnectWs(WsRequest request)
        {
            connect().ContinueWith(t => sendWsRequest().Wait()).Wait();
            Task.Run(wsReceiver);

            async Task connect()
            {
                await _ws.ConnectAsync(new Uri(_wssUri), _cts.Token)
                    .ContinueWith(t => _logger.LogInformation($"Websocket connected: {_wssUri}"),
                        TaskContinuationOptions.OnlyOnRanToCompletion);
            }

            void wsReceiver()
            {
                Thread.CurrentThread.Name = "WsRecever";
                _logger.LogInformation("Starting websocket receiving...");

                var array = new byte[1024 * 1024];
                Memory<byte> buffer = new(array);

                while (_ws.State == WebSocketState.Open)
                {
                    _evPausing.WaitOne();

                    var res = _ws.ReceiveAsync(buffer, _cts.Token).Result;
                    var json = Encoding.UTF8.GetString(array, 0, res.Count);
                    OnReceived?.Invoke(json);
                }
            }

            async Task sendWsRequest()
            {
                var json = request.ToJsonBytes();
                var rm = new ReadOnlyMemory<byte>(json, 0, json.Length);
                await _ws.SendAsync(rm, WebSocketMessageType.Binary, true, _cts.Token);
            }
        }

        #endregion


        #region ---- Rest API ----

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">IApiModel</typeparam>
        /// <param name="apiId"></param>
        /// <param name="postPath">Api URL에 추가할 경로: ex) minutes candle의 unit</param>
        /// <returns></returns>
        public T[] InvokeApi<T>(ApiId apiId, string postPath = "") where T : IApiModel, new()
        {
            if (Helper.GetApi(apiId).ResetAuthToken) _wc.SetAuthToken(_key);

            try
            {
                string json = _wc.DownloadString(Helper.GetApiPath(apiId, postPath));
                var models = JS.Deserialize<T[]>(json, _jsonOption) ?? Array.Empty<T>();
                return models;
            }
            catch (WebException ex)
            {
                _logger.LogWebException(ex, apiId);
                if (apiId == ApiId.CandleMinutes) throw;
                return Array.Empty<T>();
            }
        }

        protected void clearQueryString() => _wc.QueryString.Clear();
        protected void setQueryString(string name, string value) => _wc.QueryString[name] = value;
        protected void setQueryString(string name, int count) => _wc.QueryString[name] = count.ToString();
        protected void setQueryString(string name, CurrencyId currency, CoinId coin)
            => _wc.QueryString[name] = Helper.GetMarketId(currency, coin);

        #endregion


        #region ---- Static JSON ----

        static JsonSerializerOptions _jsonOption;
        static ClientBase()
        {
            _jsonOption = Helper.GetJsonOptions();
        }

        #endregion

    }//class
}
