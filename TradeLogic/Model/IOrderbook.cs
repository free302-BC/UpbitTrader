using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic.Model
{
    public interface IOrderbook : IApiModel
    {
        string Market { get; set; }
        List<IOrderbookUnit> OrderbookUnits { get; set; }
        long Timestamp { get; set; }
        decimal TotalAskSize { get; set; }
        decimal TotalBidSize { get; set; }
    }

}
