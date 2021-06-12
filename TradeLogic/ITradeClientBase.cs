using System;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic
{
    public interface ITradeClientBase : IDisposable
    {
        void ConnectWs(IWsRequest request);

        event Action<string>? OnWsReceived;
        void Pause(bool doPause);

        T[] InvokeApi<T>(ApiId apiId, string postPath = "") where T : IApiModel, new();

    }

}