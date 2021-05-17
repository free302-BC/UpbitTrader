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
        public double OpeningPrice { get; set; }
        public double HighPrice { get; set; }
        public double LowPrice { get; set; }
        public double TradePrice { get; set; }

        public static string Print(IEnumerable<ICandle> models)
        {
            var sb = new StringBuilder();
            foreach (var m in models) sb.AppendLine(m.ToString());
            return sb.ToString();
        }

    }//class
}
