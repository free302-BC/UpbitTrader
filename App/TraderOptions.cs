﻿using System;
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

        public new IWorkerOptions Clone()
        {
            var clone = (TraderOptions)MemberwiseClone();
            clone.CalcParam = (CalcParam)CalcParam.Clone();

            ((WorkerOptions)clone).Reload(this);

            return clone;
        }

        public new void Reload(IWorkerOptions source)
        {
            base.Reload(source);
            var src = (TraderOptions)source;
            Pausing = src.Pausing;
            CalcParam.Reload(src.CalcParam);
        }
    }
}
