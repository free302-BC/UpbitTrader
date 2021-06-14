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
    public class TickWorkerOptions : WorkerOptions
    {
        public CalcParam CalcParam { get; set; } = new();

        public new void Reload(IWorkerOptions source)
        {
            base.Reload(source);
            var src = (TickWorkerOptions)source;
            CalcParam.Reload(src.CalcParam);
        }
        public new IWorkerOptions Clone()
        {
            var clone = (TickWorkerOptions)MemberwiseClone();
            clone.CalcParam = (CalcParam)CalcParam.Clone();

            ((WorkerOptions)clone).Reload(this);

            return clone;
        }
    }
}
