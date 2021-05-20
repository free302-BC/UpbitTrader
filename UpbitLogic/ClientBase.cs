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
    public class ClientBase : IDisposable
    {
        protected ILogger _logger;
        protected WebClient _wc;
        protected const string _market = "KRW-BTC";
        protected const CurrencyId _currency = CurrencyId.KRW;
        protected const string _coin = "BTC";
        protected readonly KeyPair _key;

        public ClientBase(string accessKeyEnc, string secretKeyEnc, ILogger logger)
        {
            _logger = logger;
            _key = (accessKeyEnc.Decode(), secretKeyEnc.Decode());
            _wc = new WebClient();
            _wc.SetAuthToken(_key);
            _wc.SetAcceptance();
            _wc.SetQueryString("market", _market);
            //nvc.Add("to", "");// string | 마지막 캔들 시각 (exclusive). 포맷 : `yyyy-MM-dd'T'HH:mm:ssXXX` or `yyyy-MM-dd HH:mm:ss`. 비워서 요청 시 가장 최근 캔들  (optional) 
            //nvc.Add("convertingPriceUnit", "KRW");
        }
        public void Dispose()
        {
            _wc?.Dispose();
        }

    }//class
}
