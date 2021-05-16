using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit
{
    public class UpbitClient : IDisposable
    {
        WebClient _wc;

        public UpbitClient()
        {
            _wc = new WebClient();
            //_wc.BuildAuthToken(set);//, nvc);
            _wc.Headers["Accept"] = "application/json";
        }

        public void Dispose()
        {
            _wc?.Dispose();
        }

        public void AddQuery(NameValueCollection nvc) => _wc.QueryString.Add(nvc);





    }//class
}
