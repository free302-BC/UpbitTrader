using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic
{
    public interface IClientBase : IDisposable
    {

        #region ---- WebSocket ----

        void AddTick(CurrencyId currency, CoinId coin, (string key, object value) option = default);
        void AddOrderbook(CurrencyId currency, CoinId coin, (string key, object value) option = default);
        void AddTicker(CurrencyId currency, CoinId coin, (string key, object value) option = default);

        Task ConnectWsAsync(CancellationToken stoppingToken);
        
        event Action<TickModel>? WsTick;
        event Action<OrderbookModel>? WsOrderbook;
        event Action<TickerModel>? WsTicker;

        void Pause(bool doPause);

        #endregion


        #region ---- Rest API ----

        Task<(ApiResultCode code, T[] data)> InvokeApi<T>(ApiId apiId, string postPath = "") where T : IApiModel;

        Task<(ApiResultCode code, M[] data)> InvokeApi<M>(ApiId api, Type implType, string postPath = "") where M : IApiModel;

        #endregion


        #region ---- Interface Type Implimentation & Json ----

        Type GetImplType<I>();
        I CreateInstance<I>();
        I Deserialize<I>(string json);

        #endregion

    }//class

}