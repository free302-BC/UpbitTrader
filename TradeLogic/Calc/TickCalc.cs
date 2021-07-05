using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class TickCalc : ICalc
    {
        public static ICalc I = new TickCalc();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="prev"></param>
        /// <returns></returns>
        decimal ICalc.calcABR(ICalcModel model, ICalcModel prev)
        {
            var m = (TickModel)model;
            var a = prev.NumAsks;
            var b = prev.NumBids;
            _ = m.Dir == TradeTickDir.A ? a++ : b++;
            m.NumAsks = a;
            m.NumBids = b;
            var r = a == 0 ? 1m : Math.Round((decimal)b / a, 3);
            return model.ABR = r;
        }



        #region ---- Calc Signal ----

        const decimal _FeeRate = 0.0005m;
        const TimingSignal sN = TimingSignal.N;
        const TimingSignal sB = TimingSignal.Buy;
        const TimingSignal sH = TimingSignal.Hold;
        const TimingSignal sS = TimingSignal.Sell;

        /// <summary>
        /// MACD에 따라 TimingSignal 계산
        /// </summary>
        /// <param name="model"></param>
        /// <param name="prev"></param>
        /// <param name="param"></param>
        ///void calcSignal(ICalcModel model, ICalcModel prev, ICalcParam param)
        void ICalc.calcSignal(ICalcModel model, ICalcModel prev, ICalcParam param)
        {
            //chekc buy | sell | nop            
            if (prev.MacdOsc > param.BuyMacd)//buy
            {
                model.Signal = prev.Signal switch
                {
                    sN or sS => sB,
                    _ => sH,
                };
            }
            else if (prev.MacdOsc < param.SellMacd)//sell
            {
                model.Signal = prev.Signal switch
                {
                    sN or sS => sN,
                    _ => sS,
                };
            }
            else//nop
            {
                model.Signal = prev.Signal switch
                {
                    sN or sS => sN,
                    _ => sH,
                };
            }

            //set trade done or not
            model.TradeDone = model.Signal switch
            {
                sB or sH or sS => true,
                _ => false
            };

            //
            if (model.TradeDone)
            {
                model.Rate = model.Signal == sB ? 1.00000m : model.Value / prev.Value;
                if (model.Signal != sH) model.Rate -= _FeeRate;
                model.Rate = Math.Round(model.Rate, 5);
            }
        }

        #endregion


    }//class
}
