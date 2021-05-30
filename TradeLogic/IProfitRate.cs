using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic
{
    public interface IProfitRate
    {
        void CalcProfitRate(CandleModel[] models, decimal k);

        /// <summary>
        /// models[offset..offset+count]
        /// </summary>
        /// <param name="models"></param>
        /// <param name="k"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        void CalcProfitRate(CandleModel[] models, decimal k, int offset, int count);


    }
}
