using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.AppBase;
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Calc;

namespace Universe.Coin.App
{
    public class TickWorkerOptions : WorkerOptions
    {
        public TickWorkerOptions()
        {
            AssemblyFile = "";
            CalcParam = new CalcParam();
        }

        public string AssemblyFile { get; set; }
        public bool Pausing { get; set; }
        public ICalcParam CalcParam { get; set; }

        public new void Reload(IWorkerOptions source)
        {
            base.Reload(source);
            var src = (TickWorkerOptions)source;
            AssemblyFile = src.AssemblyFile;
            Pausing = src.Pausing;
            CalcParam.Reload(src.CalcParam);
        }
        public new IWorkerOptions Clone()
        {
            var clone = (TickWorkerOptions)MemberwiseClone();            

            clone.CalcParam = CalcParam.Clone();

            return clone;
        }
    }
}
