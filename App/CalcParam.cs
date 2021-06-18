using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Calc;

namespace Universe.Coin.App
{
    public class CalcParam: ICalcParam
    {
        public decimal FactorK { get; set; }
        //public bool ApplyMovingAvg { get; set; }
        public int WindowSize { get; set; }
        public WindowFunction WindowFunction { get; set; }
        public bool ApplyStopLoss { get; set; }
        public MacdParam MacdParam { get; set; }
        public decimal BuyMacd { get; set; }
        public decimal SellMacd { get; set; }

        public ICalcParam Clone()
        {
            var clone = (CalcParam)MemberwiseClone();
            
            return clone;
        }
        public void Reload(ICalcParam param)
        {
            FactorK = param.FactorK;
            WindowSize = param.WindowSize;
            WindowFunction = param.WindowFunction;
            ApplyStopLoss = param.ApplyStopLoss;
            MacdParam = param.MacdParam;
            BuyMacd = param.BuyMacd;
            SellMacd = param.SellMacd;
        }
    }
}
