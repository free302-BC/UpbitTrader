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
    public class BackTestOptions : WorkerOptions
    {
        public bool DoFindK { get; set; }
        public decimal Hours { get; set; }
        public bool LoadFromFile { get; set; }
        public bool PrintCandle { get; set; }
        public bool Pausing { get; set; }

        public CalcParam CalcParam { get; set; } = new();

        public new void Reload(IWorkerOptions source)
        {
            base.Reload(source);
            var src = (BackTestOptions)source;
            //if (src == null) throw new ArgumentException(
            //   $"{nameof(BackTestOptions)}.{nameof(Reload)}(): can't reload from type <{source.GetType().Name}>");

            DoFindK = src.DoFindK;
            Hours = src.Hours;
            LoadFromFile = src.LoadFromFile;
            PrintCandle = src.PrintCandle;
            CalcParam.Reload(src.CalcParam);
        }
        public new IWorkerOptions Clone()
        {
            var clone = (BackTestOptions)MemberwiseClone();
            clone.CalcParam = (CalcParam)CalcParam.Clone();

            ((WorkerOptions)clone).Reload(this);

            return clone;
        }
    }
}
