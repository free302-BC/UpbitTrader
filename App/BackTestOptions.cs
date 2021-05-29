using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.AppBase;

namespace Universe.Coin.Upbit.App
{
    public class BackTestOptions : WorkerOptions
    {
        public bool ApplyMovingAvg { get; set; }
        public int MovingAvgSize { get; set; }
        public bool ApplyStopLoss { get; set; }

        public decimal Hours { get; set; }
        public bool PrintCandle { get; set; }        

        public override void Reload(IWorkerOptions source)
        {
            base.Reload(source);
            var src = source as BackTestOptions;
            if (src == null) throw new ArgumentException(
               $"{nameof(BackTestOptions)}.{nameof(Reload)}(): can't reload from type <{source.GetType().Name}>");

            ApplyMovingAvg = src.ApplyMovingAvg;
            MovingAvgSize = src.MovingAvgSize;
            ApplyStopLoss = src.ApplyStopLoss;
            Hours = src.Hours;
            PrintCandle = src.PrintCandle;
        }
        
    }
}
