using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic.Calc
{
    public class CalcBase : ICalc
    {
        protected readonly ICalcParam _param;
        public CalcBase(ICalcParam param)
        {
            _param = param;
        }
    }
}
