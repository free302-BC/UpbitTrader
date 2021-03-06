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
        decimal Value { get; set; }//ma, macd 계산용 필드

        long Timestamp { get; set; }
        decimal MovingAvg { get; set; }
        decimal Macd { get; set; }
        decimal MacdOsc { get; set; }

        //ABR 계산용
        int NumAsks { get; set; }
        int NumBids { get; set; }
        decimal ABR { get; set; }

        decimal Target { get; set; }
        decimal BuyPrice { get; set; }
        TimingSignal Signal { get; set; }
        bool TradeDone { get; set; }

        decimal Rate { get; set; }
        decimal CumRate { get; set; }
        decimal DrawDown { get; set; }

        string CalcHeader { get; }
        string ToCalcString();
    }
}
