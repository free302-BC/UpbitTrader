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
using Universe.Utility;

namespace Universe.Coin.TradeLogic
{
    using JS = JsonSerializer;

    /// <summary>
    /// API client base
    /// </summary>
    public abstract class ClientBase : IClientBase
    {
        protected readonly KeyPair _key;
        protected readonly ILogger _logger;
        readonly JsonSerializerOptions _jsonOptions;

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

            _cts = new();
            _evPausing = new(false);

            init();
            _jsonOptions = new JsonSerializerOptions(IClientBase._jsonOptions);//clone
        }
        public void Dispose()
        {
            _wc?.Dispose();
            _ws?.Dispose();
            _cts?.Dispose();
            _evPausing?.Dispose();
        }


        /// <summary>
        /// 추가적인 객체 초기화 코드
        /// WebClient header, websocket keep-alive-time
        /// </summary>
        protected abstract void init();

        static ClientBase() => _types = new();
        static Dictionary<Type, Type> _types;
        protected static void addType<I>(Type implType) => _types[typeof(I)] = implType;

        public Type GetImplType<I>()
        {
            var type = typeof(I);
            if (!_types.ContainsKey(type))
                throw new Exception($"'{GetType().FullName}' does not impliment '{typeof(I).FullName}'"); ;
            return _types[typeof(I)];
        }
        public I CreateInstance<I>() => UvLoader.Create<I>(GetImplType<I>().FullName!);
        public I Deserialize<I>(string json)
        {
            var model = JS.Deserialize(json, GetImplType<I>(), _jsonOptions) ?? CreateInstance<I>()!;
            return (I)model;
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
        /// Connect and receive
        /// </summary>
        public async Task ConnectWsAsync(IWsRequest request)
        {
            await connect();
            await sendWsRequest();
            await wsReceiver();
            //return Task.CompletedTask;

            Task connect()
            {
                return _ws.ConnectAsync(new Uri(_wsUri), _cts.Token)
                    .ContinueWith(t => _logger.LogInformation($"Websocket connected: {_wsUri}"),
                        TaskContinuationOptions.OnlyOnRanToCompletion);//.Wait();
            }
            ValueTask sendWsRequest()
            {
                _logger.LogInformation($"Websocket sending request...");
                var json = request.ToJsonBytes();
                var rm = new ReadOnlyMemory<byte>(json, 0, json.Length);
                return _ws.SendAsync(rm, WebSocketMessageType.Binary, true, _cts.Token);
            }

            async Task wsReceiver()
            {
                _logger.LogInformation("Entering websocket receiving...");

                var array = new byte[1024 * 1024];
                Memory<byte> buffer = new(array);

                while (_ws.State == WebSocketState.Open)
                {
                    await _evPausing.WaitOneAsync();

                    var res = await _ws.ReceiveAsync(buffer, _cts.Token);
                    var json = Encoding.UTF8.GetString(array, 0, res.Count);
                    OnWsReceived?.Invoke(json);
                }
                _logger.LogInformation("Exiting websocket receiving...");
            }
        }

        #endregion


        #region ---- Rest API ----

        /// <summary>
        /// 주어진 api의 uri에 접속하여 결과를 M[]로 리턴
        /// </summary>
        /// <typeparam name="M">IApiModel</typeparam>
        /// <param name="apiId"></param>
        /// <param name="postPath">Api URL에 추가할 경로: ex) minutes candle의 unit</param>
        /// <returns></returns>
        public virtual (ApiResultCode code, M[] data) InvokeApi<M>(ApiId api, string postPath = "")
            where M : IApiModel//, new()
        {
            return InvokeApi<M>(api, typeof(M), postPath);
        }
        public virtual (ApiResultCode code, M[] data) InvokeApi<M>(ApiId api, Type implType, string postPath = "")
            where M : IApiModel//, new()
        {
            var httpUri = prepareInvoke(api, postPath);

            try
            {
                var raw = _wc.DownloadData(httpUri);
                var enc = _wc.ResponseHeaders?["Content-Encoding"] ?? "?";
                string json = enc[0] == 'g' ? Compression.Unzip(raw).ToUtf8String() : raw.ToUtf8String();
                var models = (M[])JS.Deserialize(json, implType.MakeArrayType(), _jsonOptions)!;
                return (models.Length == 0 ? ApiResultCode.OkEmpty : ApiResultCode.Ok, models);
            }
            catch (WebException ex)
            {
                _logger.LogWebException(ex);
                return (ApiResultCode.TooMany, Array.Empty<M>());
            }
        }

        /// <summary>
        /// Http Header, Query string 등 추가
        /// </summary>
        /// <param name="apiId"></param>
        /// <param name="postPath"></param>
        /// <returns>http uri</returns>
        protected abstract string prepareInvoke(ApiId apiId, string postPath);


        protected void clearQueryString() => _wc.QueryString.Clear();
        protected void setQueryString(string name, string value) => _wc.QueryString[name] = value;
        protected void addQueryString(string name, string value) => _wc.QueryString.Add(name, value);
        protected void setQueryString(string name, int count) => _wc.QueryString[name] = count.ToString();




        #endregion


    }//class
}
