using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Calc;

namespace Universe.Coin.Upbit.App
{
    public class CalcParam: ICalcParam
    {
        public decimal FactorK { get; set; }
        //public bool ApplyMovingAvg { get; set; }
        public int WindowSize { get; set; }
        public WindowFunction WindowFunction { get; set; }
        public bool ApplyStopLoss { get; set; }
        public int[] MacdWindowSizes { get; set; } = new int[3];
        public ICalcParam Clone()
        {
            var clone = (CalcParam)MemberwiseClone();
            clone.MacdWindowSizes = MacdWindowSizes.ToArray();
            return clone;
        }
        public void Reload(ICalcParam param)
        {
            FactorK = param.FactorK;
            WindowSize = param.WindowSize;
            WindowFunction = param.WindowFunction;
            ApplyStopLoss = param.ApplyStopLoss;
            param.MacdWindowSizes.CopyTo(MacdWindowSizes, 0);
        }
    }
}
