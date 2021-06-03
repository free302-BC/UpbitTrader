using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic.Calc
{
    public class CalcCandle : CalcBase, ICalcCandle
    {
        public CalcCandle(ICalcParam param) : base(param)
        {
        }
        public void CalcMacdOsc(CandleModel[] models)
        {
            int[] size = { 8, 16, 5 };
            decimal[][] avgs = new decimal[3][];
            var backupSize = _param.WindowSize;
            for (int i = 0; i < 3; i++)
            {
                _param.WindowSize = size[0];
                CalcMovingAvg(models);
                avgs[i] = models.Select(m => m.MovingAvg).ToArray();
            }
            for (int i = 0; i < models.Length; i++)
                models[i].MacdOsc = avgs[1][i] - avgs[0][i];



        }


        #region ---- Moving Average ----


        /// <summary>
        /// 가변 Window Size 기법으로 Moving Average 구함
        /// 데이터 갯수가 부족할 경우 ~ 그 갯수==Window Size
        /// </summary>
        /// <param name="models"></param>
        /// <param name="windowSize"></param>
        /// <param name="winFunc"></param>
        public void CalcMovingAvg(CandleModel[] models)
        {
            CalcMovingAvg(models, 0, models.Length);
        }
        public void CalcMovingAvg(CandleModel[] models, int offset, int count)
        {
            ICalc.CalcMovingAvg(models, offset, count, _param, m => m.Closing, (m, ma) => m.MovingAvg = ma);
        }

        #endregion


        public decimal CalcCumRate(CandleModel[] models) => CalcCumRate(models, 0, models.Length);
        public decimal CalcCumRate(CandleModel[] models, int offset, int count)
        {
            //-------------------------------------------------------------------------
            // TODO: 호출자가 이 결과를 모아 누적계산시: seed는 중복 계산된다.
            //var seed = offset > 0 ? models[offset - 1].CumRate : 1m;
            //-------------------------------------------------------------------------

            var rate = models.Skip(offset).Take(count).Aggregate(1m, (cr, m) => m.CumRate = Math.Round(cr *= m.Rate, 4));
            return Math.Round(rate, 4);
        }

        public decimal CalcDrawDown(CandleModel[] models) => CalcDrawDown(models, 0, models.Length);
        public decimal CalcDrawDown(CandleModel[] models, int offset, int count)
        {
            //-------------------------------------------------------------------------
            //TODO: max 구하기? 현재영역 or 전체
            // 현재영역의 mdd는 의미가 없음 ~ 전체 구간에서 다시 구해야 함.
            //-------------------------------------------------------------------------

            var max = decimal.MinValue;
            var sub = models.Skip(offset).Take(count);
            foreach (var m in sub)
            {
                max = max > m.CumRate ? max : m.CumRate;
                m.DrawDown = max > m.CumRate ? Math.Round(100 * (max - m.CumRate) / max, 2) : 0m;
            }

            return sub.Count() > 0 ? sub.Max(m => m.DrawDown) : 0m;
        }

    }
}
