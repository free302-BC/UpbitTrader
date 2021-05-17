using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit.Model
{
    public interface ICalcModel
    {
        public double OpeningPrice { get; set; }
        public double HighPrice { get; set; }
        public double LowPrice { get; set; }
        public double TradePrice { get; set; }
        public double NextTarget(double k) => Math.Round(TradePrice + (HighPrice - LowPrice) * k);

    }//class
}
