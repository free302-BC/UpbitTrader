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
        decimal MovingAvg { get; set; }
        decimal MacdOsc { get; set; }

        decimal Target { get; set; }
        TimingSignal Signal { get; set; }
        bool TradeDone { get; set; }

        decimal Rate { get; set; }
        decimal CumRate { get; set; }
        decimal DrawDown { get; set; }

        const decimal FeeRate = 0.0005m * 2m;
    }
}
