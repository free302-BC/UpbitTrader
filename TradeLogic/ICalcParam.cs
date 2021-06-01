using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic
{
    public interface ICalcParam
    {

        public decimal FactorK { get;  }

        public bool ApplyMovingAvg { get;  }
        public int WindowSize { get;  }
        public WindowFunction WindowFunction { get;  }

        public bool ApplyStopLoss { get;  }

    }//interface
}
