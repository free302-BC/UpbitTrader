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
            //nvc.Add("to", "");// string | 마지막 캔들 시각 (exclusive). 포맷 : `yyyy-MM-dd'T'HH:mm:ssXXX` or `yyyy-MM-dd HH:mm:ss`. 비워서 요청 시 가장 최근 캔들  (optional) 
            //nvc.Add("convertingPriceUnit", "KRW");
        }
        public void Dispose() => _wc?.Dispose();

        public List<T> InvokeApi<T>(ApiId apiId, Action? queryAction = null) where T: class, new()
        {
            _wc.QueryString.Clear();
            if (Helper.GetApi(apiId).ResetAuthToken) _wc.SetAuthToken(_key);
            queryAction?.Invoke();

            try
            {
                string response = _wc.DownloadString(Helper.GetApiPath(apiId));
                var book = JsonConvert.DeserializeObject<List<T>>(response) ?? new List<T>();
                return book;
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
