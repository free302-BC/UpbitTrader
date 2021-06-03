using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.AppBase;
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Calc;

namespace Universe.Coin.Upbit.App
{
    public class TraderOptions : WorkerOptions, ICalcParam
    {
        public int WindowSize { get; set; }
        public WindowFunction WindowFunction { get; set; }
        public int[] MacdWindowSizes { get; set; } = new[] { 8, 16, 5 };
        public decimal FactorK { get; set; }
        public bool ApplyStopLoss { get; set; }

        public ICalcParam Clone()
        {
            var clone = (TraderOptions)MemberwiseClone();
            clone.MacdWindowSizes = MacdWindowSizes.ToArray();
            return clone;
        }

        public override void Reload(IWorkerOptions source)
        {
            base.Reload(source);
            var src = source as TraderOptions;
            if (src == null) throw new ArgumentException(
               $"{nameof(TraderOptions)}.{nameof(Reload)}(): can't reload from type <{source.GetType().Name}>");

            FactorK = src.FactorK;
            WindowSize = src.WindowSize;
            WindowFunction = src.WindowFunction;
            ApplyStopLoss = src.ApplyStopLoss;
            MacdWindowSizes = src.MacdWindowSizes.ToArray();
        }
    }
}
