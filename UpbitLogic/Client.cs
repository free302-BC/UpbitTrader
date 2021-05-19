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

namespace Universe.Coin.Upbit
{
    public class Client : ClientBase
    {
        public Client(string token, ILogger logger) : base(token, logger) { }

        [return: NotNull]
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

        public Orderbook ApiOrderbook()
        {
            var api = ApiId.OrderOrderbook;
            _wc.RemoveQueryString("count");
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

        public Ticker ApiTicker(string market = _market)
        {
            var api = ApiId.TradeTicker;
            _wc.QueryString.Clear();
            _wc.SetQueryString("markets", market);
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

        public List<Account> ApiAccount()
        {
            var api = ApiId.AccountInfo;
            _wc.QueryString.Clear();
            try
            {
                string response = _wc.DownloadString(Helper.GetApiUrl(api));
                var list = JsonConvert.DeserializeObject<List<Account>>(response) ?? new List<Account>();
                return list;
            }
            catch (WebException ex)
            {
                _logger.LogWebException(ex, api);
                return new List<Account>();
            }
        }

        public List<MarketInfo> ApiMarketInfo()
        {
            var api = ApiId.MarketInfoAll;
            _wc.QueryString.Clear();
            try
            {
                string response = _wc.DownloadString(Helper.GetApiUrl(api));
                var list = JsonConvert.DeserializeObject<List<MarketInfo>>(response) ?? new List<MarketInfo>();
                return list;
            }
            catch (WebException ex)
            {
                _logger.LogWebException(ex, api);
                return new List<MarketInfo>();
            }
        }




    }//class
}
