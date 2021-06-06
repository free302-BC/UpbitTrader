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
    public partial class Client : ClientBase
    {
        public Account[] ApiAccount() => InvokeApi<Account>(ApiId.AccountInfo);

        public decimal GetBalance(CurrencyId currency)
        {
            var accounts = ApiAccount();
            var krw = accounts.FirstOrDefault(a => a.Currency == currency.ToString());
            return krw?.Balance ?? 0m;
        }
        public decimal GetBalance(CoinId coin)
        {
            var accounts = ApiAccount();
            var krw = accounts.FirstOrDefault(a => a.Currency == coin.ToString());
            return krw?.Balance ?? 0m;
        }


    }//class
}
