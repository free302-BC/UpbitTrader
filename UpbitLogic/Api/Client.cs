using Microsoft.Extensions.Logging;
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
using Universe.Coin.TradeLogic;
using Universe.Coin.Upbit.Model;
using Universe.Utility;

namespace Universe.Coin.Upbit
{
    public partial class Client : ClientBase, IClient
    {
        public Client(string accessKey, string secretKey, ILogger logger) :
            base(accessKey, secretKey, logger)
        { }

        public MarketInfo[] ApiMarketInfo() => InvokeApi<MarketInfo>(ApiId.MarketInfoAll);

        public string ApiTest(IApiParam param)
        {
            return param.ToString()!;
        }
    }//class
}
