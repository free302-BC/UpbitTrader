﻿using Microsoft.Extensions.Logging;
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
        public Client(string accessKeyEnc, string secretKeyEnc, ILogger logger) : 
            base(accessKeyEnc, secretKeyEnc, logger) { }

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
