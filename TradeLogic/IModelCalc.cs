using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic
{
    public interface IModelCalc
    {
        static decimal[] _gaussian_coef =
        {
            1.2533E+0m,3.1205E+0m,5.0997E+0m,7.1270E+0m,9.1776E+0m,1.1241E+1m,1.3313E+1m,1.5390E+1m,1.7471E+1m,1.9555E+1m,
            2.1640E+1m,2.3727E+1m,2.5816E+1m,2.7905E+1m,2.9995E+1m,3.2085E+1m,3.4177E+1m,3.6268E+1m,3.8360E+1m,4.0452E+1m,
            4.2545E+1m,4.4638E+1m,4.6731E+1m,4.8824E+1m,5.0917E+1m,5.3011E+1m,5.5105E+1m,5.7198E+1m,5.9292E+1m,6.1386E+1m,
            6.3480E+1m,6.5575E+1m,6.7669E+1m,6.9763E+1m,7.1857E+1m,7.3952E+1m,7.6046E+1m,7.8141E+1m,8.0235E+1m,8.2330E+1m,
            8.4424E+1m,8.6519E+1m,8.8614E+1m,9.0708E+1m,9.2803E+1m,9.4898E+1m,9.6993E+1m,9.9087E+1m,1.0118E+2m,1.0328E+2m,
            1.0537E+2m,1.0747E+2m,1.0956E+2m,1.1166E+2m,1.1375E+2m,1.1585E+2m,1.1794E+2m,1.2004E+2m,1.2213E+2m,1.2423E+2m,
            1.2632E+2m,1.2842E+2m,1.3051E+2m,1.3261E+2m,1.3470E+2m,1.3680E+2m,1.3889E+2m,1.4099E+2m,1.4308E+2m,1.4518E+2m,
            1.4727E+2m,1.4937E+2m,1.5146E+2m,1.5356E+2m,1.5565E+2m,1.5775E+2m,1.5984E+2m,1.6194E+2m,1.6403E+2m,1.6613E+2m,
            1.6822E+2m,1.7032E+2m,1.7241E+2m,1.7451E+2m,1.7660E+2m,1.7870E+2m,1.8079E+2m,1.8289E+2m,1.8498E+2m,1.8708E+2m,
            1.8917E+2m,1.9127E+2m,1.9337E+2m,1.9546E+2m,1.9756E+2m,1.9965E+2m,2.0175E+2m,2.0384E+2m,2.0594E+2m,2.0803E+2m,
        };
        static decimal gauss(double x, int count)
        {
            var exp = Math.Exp(-2 * (x - count) / count * (x - count) / count);
            return (decimal)(0.79788456 / count * exp);
        }
        public static void CalcMovingAvg(CandleModel[] models, int size, WindowFunction wf = WindowFunction.Flat)
        {
            var f = new decimal[size][];
            for (int i = 0; i < size; i++)
            {
                int count = i + 1;// Math.Min(i + 1, size);
                f[i] = new decimal[count];

                if (wf == WindowFunction.Flat)
                    Array.Fill(f[i], 1m);//flat wf
                if (wf == WindowFunction.Linear)
                    for (int j = 0; j < count; j++)
                        f[i][j] = 2m * j / (1m + count);
                if (wf == WindowFunction.Gaussian)
                    for (int j = 0; j < count; j++)
                        f[i][j] = gauss(j, count) * _gaussian_coef[i];//TODO: 
            }

            for (int i = 0; i < models.Length; i++)
            {
                var sum = 0m;
                int count = Math.Min(i + 1, size);// i + 1 >= size ? size : i + 1;

                for (int j = i; j > (i - count); j--) sum += models[j].Closing;

                models[i].MovingAvg = sum / count;
            }
        }

        public static decimal CalcCumRate(CandleModel[] models) => CalcCumRate(models, 0, models.Length);
        public static decimal CalcCumRate(CandleModel[] models, int offset, int count)
        {
            var seed = offset > 0 ? models[offset - 1].CumRate : 1m;
            var rate = models.Skip(offset).Take(count).Aggregate(seed, (cr, m) => m.CumRate = Math.Round(cr *= m.Rate, 4));
            return Math.Round(rate, 4);
        }

        public static decimal CalcDrawDown(CandleModel[] models) => CalcDrawDown(models, 0, models.Length);
        public static decimal CalcDrawDown(CandleModel[] models, int offset, int count)
        {
            //TODO: max 구하기? 현재영역 or 전체
            var max = decimal.MinValue;
            var sub = models.Skip(offset).Take(count);
            foreach (var m in sub)
            {
                max = max > m.CumRate ? max : m.CumRate;
                m.DrawDown = max > m.CumRate ? Math.Round(100 * (max - m.CumRate) / max, 2) : 0m;
            }

            return sub.Count() > 0 ? sub.Max(m => m.DrawDown) : 0m;
        }


        //void calcRate_StopLoss(CandleModel prev, decimal k)
        //{
        //    Target = Math.Round(Opening + prev.Delta * k, 2);

        //    var sellPrice = Target * 0.99m > Low ? Math.Max(Target * 0.985m, Low) : Closing;
        //    //하락후 회복시 미반영

        //    Rate = (High > Target) ? Math.Round(sellPrice / Target - FeeRate, 4) : 1.0m;
        //}

    }
}
