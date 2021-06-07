using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic.Model
{
    public interface IOrderbook : IApiModel
    {
        string Market { get; set; }
        
        IEnumerable<IOrderbookUnit> OrderbookUnits { get; set; }

        long Timestamp { get; set; }
        decimal TotalAskSize { get; set; }
        decimal TotalBidSize { get; set; }

        static JsonSerializerOptions _jsonOption;
        static IOrderbook()
        {
            _jsonOption = new JsonSerializerOptions();
            _jsonOption.IncludeFields = true;
            _jsonOption.WriteIndented = true;
            _jsonOption.PropertyNameCaseInsensitive = false;
        }

    }//class
}
