﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic
{
    public class TradeOptionsBase : ITradeOptions
    {
        public string AccessKey { get; set; } = "";
        public string SecretKey { get; set; } = "";

    }
}
