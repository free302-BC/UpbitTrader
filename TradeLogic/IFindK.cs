﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic
{
    using FindList = List<(int trades, decimal k, decimal rate, decimal mdd)>;

    public interface IFindK
    {
        static (int trades, decimal k, decimal rate, decimal mdd) 
            findK(CandleModel[] models, int offset, int count, bool applyMovingAvg, StringBuilder? sb = null)
        {
            if (models.Length == 0) return (0, 0m, 0m, 0m);
            else
            {
                var list = new FindList();
                for (decimal k = 0.1m; k <= 2m; k += 0.1m)
                {
                    var (trades, rate, mdd) = IBackTest.backTest(models, k, offset, count, applyMovingAvg);
                    list.Add((trades, k, rate, mdd));
                    sb?.AppendLine($"{k,6:F2}: {(rate - 1) * 100,10:F2}%, {mdd,10:F2}%");
                }
                var maxRate = list.Max(x => x.rate);
                var max = list.First(x => x.rate == maxRate);
                return max;
            }
        }
    }
}
