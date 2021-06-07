using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;
using Universe.Coin.Upbit.Model;
using Xunit;

namespace UnitTester
{
    //using JU = Utf8Json.JsonSerializer;
    using JU = System.Text.Json.JsonSerializer;

    public class Utf8JsonTester
    {
        [Fact] void test()
        {
            var json = File.ReadAllText("orderbook.json");
            var response = JU.Deserialize<Orderbook>(json);
            var type = response.OrderbookUnits;
        }

    }
}
