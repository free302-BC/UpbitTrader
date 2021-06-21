﻿using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic
{
    public interface IClientBase : IDisposable
    {

        #region ---- WebSocket ----
        Task ConnectWsAsync(IWsRequest request);
        event Action<string>? OnWsReceived;
        void Pause(bool doPause);
        #endregion


        #region ---- Rest API ----

        (ApiResultCode code, T[] data) InvokeApi<T>(ApiId apiId, string postPath = "") where T : IApiModel;

        (ApiResultCode code, M[] data) InvokeApi<M>(ApiId api, Type implType, string postPath = "") where M : IApiModel;

        #endregion


            #region ---- Json Options ----

        static IClientBase()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                IncludeFields = true,
                WriteIndented = true,
                PropertyNameCaseInsensitive = false,
                NumberHandling = JsonNumberHandling.AllowReadingFromString 
                | JsonNumberHandling.AllowNamedFloatingPointLiterals
            };
            _jsonOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            _jsonOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.HangulSyllables);
            //initJsonOption(_jsonOption);
        }

        //JsonSerializerOptions JsonOptions { get; }
        protected static readonly JsonSerializerOptions _jsonOptions;

        Type GetImplType<I>();
        I CreateInstance<I>();
        I Deserialize<I>(string json);

        #endregion

    }//class

}