using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic.Model
{
    /// <summary>
    /// 계산용 모델의 기본 인터페이스
    /// </summary>
    public interface ICalcModel : IViewModel
    {
        //계산용
        public decimal MovingAvg { get; set; }
        public decimal MacdOsc { get; set; }
        public decimal Target { get; set; }
        public decimal Rate { get; set; }
        public decimal CumRate { get; set; }
        public decimal DrawDown { get; set; }

        const decimal FeeRate = 0.0005m * 2m;
    }
}
