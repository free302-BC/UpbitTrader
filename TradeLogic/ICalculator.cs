using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic
{
    /// <summary>
    /// 모든 계산 interface의 기본
    /// </summary>
    public interface ICalculator
    {

    }

    public class CalculatorBase : ICalculator
    {
        protected readonly ICalcParam _param;
        public CalculatorBase(ICalcParam param)
        {
            _param = param;
        }
    }
}
