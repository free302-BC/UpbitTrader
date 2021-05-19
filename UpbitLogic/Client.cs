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
                var list = JsonConvert.DeserializeObject<List<M>>(response)! ?? new List<M>();
                return list;
            }
            catch (WebException ex)
            {
                _logger.LogWebException(ex, api);
                return new List<M>();
            }
        }

        public List<Orderbook> ApiOrderbook()
        {
            var api = ApiId.OrderOrderbook;
            _wc.RemoveQueryString("count");
            try
            {
                string response = _wc.DownloadString(Helper.GetApiUrl(api));
                var list = JsonConvert.DeserializeObject<List<Orderbook>>(response) ?? new List<Orderbook>();
                return list;
            }
            catch (WebException ex)
            {
                _logger.LogWebException(ex, api);
                return new List<Orderbook>();
            }
        }

        public List<Ticker> ApiTicker()
        {
            var api = ApiId.TradeTicker;
            _wc.QueryString.Clear();
            _wc.SetQueryString("markets", _market);
            try
            {
                string response = _wc.DownloadString(Helper.GetApiUrl(api));
                var list = JsonConvert.DeserializeObject<List<Ticker>>(response)! ?? new List<Ticker>();
                return list;
            }
            catch (WebException ex)
            {
                _logger.LogWebException(ex, api);
                return new List<Ticker>();
            }
        }

    }//class
}
