using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Universe.Coin.Upbit.Model;

namespace Universe.Coin.Upbit
{
    public abstract class ClientBase : IDisposable
    {
        protected ILogger _logger;
        protected WebClient _wc;
        protected const string _market = "KRW-BTC";
        protected readonly KeyPair _key;

        public ClientBase(string accessKeyEnc, string secretKeyEnc, ILogger logger)
        {
            _logger = logger;
            _key = (accessKeyEnc.Decode(), secretKeyEnc.Decode());
            _wc = new WebClient();
            _wc.SetAuthToken(_key);
            _wc.SetAcceptance();
        }
        public void Dispose() => _wc?.Dispose();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">IApiModel</typeparam>
        /// <param name="apiId"></param>
        /// <param name="postPath">Api URL에 추가할 경로: ex) minutes candle의 unit</param>
        /// <returns></returns>
        public List<T> InvokeApi<T>(ApiId apiId, string postPath = "") where T : IApiModel, new()
        {
            //if (clearQuery) _wc.QueryString.Clear();
            //queryAction?.Invoke();
            if (Helper.GetApi(apiId).ResetAuthToken) _wc.SetAuthToken(_key);

            try
            {
                string json = _wc.DownloadString(Helper.GetApiPath(apiId, postPath));
                var modles = JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
                return modles;
            }
            catch (WebException ex)
            {
                _logger.LogWebException(ex, apiId);
                return new List<T>();
            }
        }
        
        protected void clearQueryString() => _wc.QueryString.Clear();
        protected void setQueryString(string name, string value) => _wc.QueryString[name] = value;
        protected void setQueryString(string name, int count) => _wc.QueryString[name] = count.ToString();
        protected void setQueryString(string name, CurrencyId currency, CoinId coin)
            => _wc.QueryString[name] = Helper.GetMarketId(currency, coin);


    }//class
}
