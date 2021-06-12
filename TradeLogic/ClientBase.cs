﻿using System;
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
using Universe.Utility;

namespace Universe.Coin.TradeLogic
{
    using JS = JsonSerializer;

    /// <summary>
    /// API client base
    /// </summary>
    public abstract class ClientBase : ITradeClientBase
    {
        protected readonly KeyPair _key;
        protected readonly ILogger _logger;

        readonly string _wsUri;
        protected readonly WebClient _wc;
        protected readonly ClientWebSocket _ws;
        readonly CancellationTokenSource _cts;

        public ClientBase(string wsUri, string accessKey, string secretKey, ILogger logger)
        {
            _wsUri = wsUri;
            _logger = logger;
            _key = (accessKey, secretKey);

            _wc = new();
            _ws = new();
            init();

            _cts = new();
            _evPausing = new(false);
        }

        protected abstract void init();

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
        public event Action<string>? OnWsReceived;

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
        public void ConnectWs(IWsRequest request)
        {
            connectAsync();
            sendWsRequest();
            Task.Run(wsReceiver);

            void connectAsync()
            {
                _ws.ConnectAsync(new Uri(_wsUri), _cts.Token)
                    .ContinueWith(t => _logger.LogInformation($"Websocket connected: {_wsUri}"),
                        TaskContinuationOptions.OnlyOnRanToCompletion).Wait();
            }
            ValueTask sendWsRequest()
            {
                var json = request.ToJsonBytes();
                var rm = new ReadOnlyMemory<byte>(json, 0, json.Length);
                return _ws.SendAsync(rm, WebSocketMessageType.Binary, true, _cts.Token);
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
                    OnWsReceived?.Invoke(json);
                }
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
        public virtual T[] InvokeApi<T>(ApiId api, string postPath = "") where T : IApiModel, new()
        {
            var httpUri = prepareInvoke(api, postPath);

            try
            {
                string json = _wc.DownloadString(httpUri);
                var models = JS.Deserialize<T[]>(json, _jsonOption) ?? Array.Empty<T>();
                return models;
            }
            catch (WebException ex)
            {
                _logger.LogWebException(ex);
                return Array.Empty<T>();
            }
        }
        protected abstract string prepareInvoke(ApiId apiId, string postPath);


        protected void clearQueryString() => _wc.QueryString.Clear();
        protected void setQueryString(string name, string value) => _wc.QueryString[name] = value;
        protected void addQueryString(string name, string value) => _wc.QueryString.Add(name, value);
        protected void setQueryString(string name, int count) => _wc.QueryString[name] = count.ToString();

        #endregion


        #region ---- Static JSON ----

        static JsonSerializerOptions _jsonOption;
        static ClientBase()
        {
            _jsonOption = getJsonOptions();
        }
        static JsonSerializerOptions getJsonOptions()
        {
            var opt = new JsonSerializerOptions();
            opt.IncludeFields = true;
            opt.WriteIndented = true;
            opt.PropertyNameCaseInsensitive = false;
            opt.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            opt.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.HangulSyllables);
            return opt;
        }
        #endregion

    }//class
}