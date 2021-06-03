using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic.Model
{
    public interface IOrderbookUnit
    {
        decimal AskPrice { get; set; }
        decimal AskSize { get; set; }
        decimal BidPrice { get; set; }
        decimal BidSize { get; set; }

        string ToString();
    }

}
