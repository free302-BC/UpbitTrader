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
        public TickWorkerOptions()
        {
            AssemblyFiles = new();
            CalcParam = new CalcParam();
        }

        public bool Pausing { get; set; }
        public Dictionary<string, string> AssemblyFiles { get; set; }
        public ICalcParam CalcParam { get; set; }

        public new void Reload(IWorkerOptions source)
        {
            base.Reload(source);
            var src = (TickWorkerOptions)source;
            Pausing = src.Pausing;
            CalcParam.Reload(src.CalcParam);
            foreach (var key in src.AssemblyFiles.Keys) AssemblyFiles[key] = src.AssemblyFiles[key];
        }
        public new IWorkerOptions Clone()
        {
            var clone = (TickWorkerOptions)MemberwiseClone();

            clone.AssemblyFiles = new();
            foreach (var key in AssemblyFiles.Keys) clone.AssemblyFiles[key] = AssemblyFiles[key];

            clone.CalcParam = CalcParam.Clone();

            return clone;
        }
    }
}
