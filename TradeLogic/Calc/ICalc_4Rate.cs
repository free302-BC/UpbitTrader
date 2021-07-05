using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic.Calc
{
    using M = ICalcModel;

    public partial interface ICalc
    {
        /// <summary>
        /// 주어진 ICalcParam을 이용하여 Profit Rate를 계산
        /// </summary>
        /// <param name="models"></param>
        /// <param name="param"></param>
        public void CalcProfitRate(M[] models, ICalcParam param)
        {
            //calcSignal(models[0], M.Empty, param);
            for (int i = 1; i < models.Length; i++)
                calcSignal(models[i], models[i - 1], param);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CalcProfitRate(M[] models, ICalcParam param, int index)
        {
            if (index > 0) calcSignal(models[index], models[index - 1], param);
        }

        protected void calcSignal(M model, M prev, ICalcParam param);



        #region ---- TEST ----

        public static void MacdProfitRate()
        {
            int[] windowSizes = { 8, 16, 5 };

            
        }
        #endregion

    }
}
