using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic
{
    public interface ITradeLogic
    {
        void CalcProfitRate(CandleModel model, CandleModel prev, decimal k, decimal ma = 0);

        void CalcProfitRate(IList<CandleModel> models, decimal k, decimal ma = 0);
        

    }



}
