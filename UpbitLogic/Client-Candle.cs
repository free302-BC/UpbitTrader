using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Universe.Coin.Upbit.Model;
using Universe.Utility;

namespace Universe.Coin.Upbit
{
    public partial class Client : ClientBase
    {
        public List<M> ApiCandle<M>(int count = 15) where M : ICandle
        {
            _wc.SetQueryString("count", count.ToString());
            var api = ApiId.CandleDays;
            try
            {
                string response = _wc.DownloadString(Helper.GetApiUrl(api));
                var list = JsonConvert.DeserializeObject<List<M>>(response) ?? new List<M>();
                return list;
            }
            catch (WebException ex)
            {
                _logger.LogWebException(ex, api);
                return new List<M>();
            }
        }

        public Ticker ApiTicker(CurrencyId currency = CurrencyId.KRW, CoinId coin = CoinId.BTC)
        {
            var api = ApiId.TradeTicker;
            _wc.QueryString.Clear();
            _wc.SetQueryString("markets", currency, coin);
            try
            {
                string response = _wc.DownloadString(Helper.GetApiUrl(api));
                var list = JsonConvert.DeserializeObject<List<Ticker>>(response)?.FirstOrDefault() ?? new Ticker();
                return list;
            }
            catch (WebException ex)
            {
                _logger.LogWebException(ex, api);
                return new Ticker();
            }
        }

        public Orderbook ApiOrderbook(CurrencyId currency = CurrencyId.KRW, CoinId coin = CoinId.BTC)
        {
            var api = ApiId.OrderOrderbook;
            _wc.QueryString.Clear();
            _wc.SetQueryString("markets", currency, coin);
            try
            {
                string response = _wc.DownloadString(Helper.GetApiUrl(api));
                var book = JsonConvert.DeserializeObject<List<Orderbook>>(response)?.FirstOrDefault() ?? new Orderbook();
                return book;
            }
            catch (WebException ex)
            {
                _logger.LogWebException(ex, api);
                return new Orderbook();
            }
        }

    }//class
}
