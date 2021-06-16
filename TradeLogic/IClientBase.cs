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
        void ConnectWs(IWsRequest request);
        event Action<string>? OnWsReceived;
        void Pause(bool doPause);
        #endregion


        #region ---- Rest API ----

        T[] InvokeApi<T>(ApiId apiId, string postPath = "") where T : IApiModel;

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