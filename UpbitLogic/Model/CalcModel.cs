using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit.Model
{
    public class CalcModel
    {
        public double Opening;
        public double High;
        public double Low;
        public double Closing;
        public double Target;
        public double Rate;
        public double NextTarget;
        public double K;

        public CalcModel(ICalcModel candle, double k)
        {
            Opening = candle.OpeningPrice / 10000.0;
            High = candle.HighPrice / 10000.0;
            Low = candle.LowPrice / 10000.0;
            Closing = candle.TradePrice / 10000.0;
            K = k;
            NextTarget = candle.NextTarget(k) / 10000.0;
        }
        public double CalcRate(double target)
        {
            Target = target;
            Rate = (High > target) ? Closing / target - 0.0015 : 1.0;
            return Rate;
        }
        public override string ToString() => $"{Opening,10} {Target,10} {High,10} {Closing,10} : {Rate,10}";

    }//class
}
