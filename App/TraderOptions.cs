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
    public class TraderOptions : WorkerOptions
    {
        public bool Pausing { get; set; }
        public CalcParam CalcParam { get; set; } = new();

        public TraderOptions Clone()
        {
            var clone = (TraderOptions)MemberwiseClone();
            clone.CalcParam = (CalcParam)CalcParam.Clone();
            return clone;
        }

        public override void Reload(IWorkerOptions source)
        {
            base.Reload(source);
            var src = source as TraderOptions;
            if (src == null) throw new ArgumentException(
               $"{nameof(TraderOptions)}.{nameof(Reload)}(): can't reload from type <{source.GetType().Name}>");

            CalcParam.Reload(src.CalcParam);
        }
    }
}
