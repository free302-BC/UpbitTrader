using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic.Calc
{
    /// <summary>
    /// ITicker 모델에 대한 계산 알고리즘 구현
    /// </summary>
    public class TickerCalc : ICalc
    {
        const decimal _FeeRate = 0.0005m;
        public static ICalc I = new TickerCalc();

        /// <summary>
        /// MACD에 따라 TimingSignal 계산
        /// </summary>
        /// <param name="model"></param>
        /// <param name="prev"></param>
        /// <param name="param"></param>
        ///void calcSignal(ICalcModel model, ICalcModel prev, ICalcParam param)
        void ICalc.CalcSignal(ICalcModel model, ICalcModel prev, ICalcParam param)
        {
            //chekc buy | sell | nop
            var signal = TimingSignal.N;
            if (prev.MacdOsc > param.BuyMacd) signal = TimingSignal.Buy;
            if (prev.MacdOsc < param.SellMacd) signal = TimingSignal.Sell;

            model.Signal = prev.Signal switch
            {
                TimingSignal.N or TimingSignal.Sell
                    => signal == TimingSignal.Buy ? signal : TimingSignal.N,
                _ => signal == TimingSignal.Sell ? signal : TimingSignal.Hold,
            };
            //
            model.TradeDone = model.Signal == TimingSignal.Hold || model.Signal == TimingSignal.Sell;

            //
            if (model.TradeDone)
            {
                model.Rate = model.Value/ prev.Value;
                if (model.Signal == TimingSignal.Buy || model.Signal == TimingSignal.Sell)
                    model.Rate -= _FeeRate;
                model.Rate = Math.Round(model.Rate, 4);
            }
        }


    }//class
}
