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
        public List<Account> ApiAccount()
        {
            var api = ApiId.AccountInfo;
            _wc.QueryString.Clear();
            _wc.SetAuthToken(_key);
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

        public double GetBalance(CurrencyId currencyId)
        {
            var accounts = ApiAccount();
            var krw = accounts.FirstOrDefault(a => a.Currency == currencyId.ToString());
            return krw?.Balance?.To<double>() ?? 0.0;
        }
        public double GetBalance(string coinId)
        {
            var accounts = ApiAccount();
            var krw = accounts.FirstOrDefault(a => a.Currency == coinId);
            return krw?.Balance?.To<double>() ?? 0.0;
        }


    }//class
}
