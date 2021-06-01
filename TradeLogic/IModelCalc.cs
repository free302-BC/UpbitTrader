using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic
{
    public interface IModelCalc : ICalculator
    {
        void CalcMovingAvg(CandleModel[] models);
        void CalcMovingAvg(CandleModel[] models, int offset, int count);

        decimal CalcCumRate(CandleModel[] models);
        decimal CalcCumRate(CandleModel[] models, int offset, int count);

        decimal CalcDrawDown(CandleModel[] models);
        decimal CalcDrawDown(CandleModel[] models, int offset, int count);        

    }
}
