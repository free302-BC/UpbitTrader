using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit.Model
{
    public interface ICandle
    {
        public string CandleDateTimeKst { get; set; }
        public decimal OpeningPrice { get; set; }
        public decimal HighPrice { get; set; }
        public decimal LowPrice { get; set; }
        public decimal TradePrice { get; set; }

        public static string Print(IEnumerable<ICandle> models)
        {
            var sb = new StringBuilder();
            foreach (var m in models) sb.AppendLine(m.ToString());
            return sb.ToString();
        }

    }//class
}
