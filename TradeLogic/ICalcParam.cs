﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic
{
    public interface ICalcParam
    {
        public decimal FactorK { get; set; }

        public int WindowSize { get; set; }

        public WindowFunction WindowFunction { get; set; }

        public bool ApplyStopLoss { get; set; }

        public ICalcParam Clone();
        

    }//interface
}
