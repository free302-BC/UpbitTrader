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

        //public decimal FactorK { get; set; }
        ////public bool ApplyMovingAvg { get; set; }
        //public int WindowSize { get; set; }
        //public WindowFunction WindowFunction { get; set; }
        //public bool ApplyStopLoss { get; set; }
        //public int[] MacdWindowSizes { get; set; } = new int[0];

        public override void Reload(IWorkerOptions source)
        {
            base.Reload(source);
            var src = source as BackTestOptions;
            if (src == null) throw new ArgumentException(
               $"{nameof(BackTestOptions)}.{nameof(Reload)}(): can't reload from type <{source.GetType().Name}>");

            DoFindK = src.DoFindK;
            Hours = src.Hours;
            LoadFromFile = src.LoadFromFile;
            PrintCandle = src.PrintCandle;
            CalcParam.Reload(src.CalcParam);
        }

        public BackTestOptions Clone()
        {
            var clone = (BackTestOptions)MemberwiseClone();
            clone.CalcParam = (CalcParam)CalcParam.Clone();
            return clone;
        }
    }
}
