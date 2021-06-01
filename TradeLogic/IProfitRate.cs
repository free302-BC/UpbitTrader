using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic
{
    public interface IProfitRate : ICalculator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="models"></param>
        void CalcProfitRate(CandleModel[] models);

        /// <summary>
        /// models[offset..offset+count]
        /// </summary>
        /// <param name="models"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        void CalcProfitRate(CandleModel[] models, int offset, int count);


    }
}
