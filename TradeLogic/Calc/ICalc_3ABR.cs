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
        /// 주어진 ICalcParam을 이용하여 체결강도 AskBidRatio를 계산
        /// </summary>
        /// <param name="models"></param>
        /// <param name="param"></param>
        public void CalcABR(M[] models)
        {
            for (int i = 1; i < models.Length; i++)
                calcABR(models[i], models[i-1]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CalcABR(M[] models, int index)
        {
            if(index > 0) calcABR(models[index], models[index - 1]);
        }

        /// <summary>
        /// ABR을 계산하여 model.ABR에 저장
        /// </summary>
        /// <param name="model"></param>
        /// <param name="index"></param>
        /// <param name="seed">이전까지 계산된 ABR</param>
        /// <returns></returns>
        protected decimal calcABR(M model, M prev);



        #region ---- TEST ----

        public static void TestABR()
        {
            int[] windowSizes = { 8, 16, 5 };

            
        }
        #endregion

    }
}
