using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic.Calc
{
    /// <summary>
    /// IViewModel에 대한 기본 계산 알고리즘 구현
    /// </summary>
    public partial interface ICalc
    {
        /// <summary>
        /// ICalcModel[]에 대한 moving average 계산
        /// 가변 Window Size 기법으로 Moving Average 구함
        /// 데이터 갯수가 부족할 경우 ~ 그 갯수==Window Size
        /// </summary>
        /// <typeparam name="VM"></typeparam>
        /// <param name="models"></param>
        /// <param name="param"></param>
        /// <param name="getter"></param>
        static void CalcMovingAvg<VM>(VM[] models, ICalcParam param, Func<VM, decimal> getter)
            where VM : ICalcModel
        {
            if (param.WindowFunction == WindowFunction.None) return;

            var values = models.Select(v => getter(v)).ToArray();
            var avgs = calcMovingAvg(values, param.WindowSize, param.WindowFunction);
            for (int i = 0; i < avgs.Length; i++) models[i].MovingAvg = avgs[i];
        }

        /// <summary>
        /// 가변 Window Size 기법으로 Moving Average 구함
        /// 데이터 갯수가 부족할 경우 ~ 그 갯수==Window Size
        /// </summary>
        /// <returns></returns>
        private static decimal[] calcMovingAvg(decimal[] values, int windowSize, WindowFunction windowFunction)
        {
            var winFuncs = _winFuncs[windowFunction];

            //TODO: offset ~ offset 이전 데이터 계산포함?
            var ma = new decimal[values.Length];
            for (int i = 0; i < values.Length; i++)//i ~ models index
            {
                ma[i] = calcMovingAvg(values, winFuncs[Math.Min(i + 1, windowSize)], i);
            }
            return ma;
        }

        /// <summary>
        /// values의 index번째 원소에 대응하는 MA를 weights를 이용하여 계산
        /// </summary>
        /// <param name="values"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        static decimal calcMovingAvg(decimal[] values, decimal[] weights, int index)
        {
            var len = weights.Length;
            var j0 = index - (len - 1);//start index of effective data(used to calcualte)

            var src = new ArraySegment<decimal>(values, j0, len);
            var ma = Math.Round(weights.Zip(src, (w, s) => w * s).Average(), 2);

            return ma;
        }



        #region ---- TEST ----

        public static void MovingAvgTest()
        {
            var ma = calcMovingAvg(_src, 5, WindowFunction.Linear);
            save(_src, ma, "moving_average_2.txt");

            static void save(decimal[] values, decimal[] ma, string fileName)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"src\tma");
                for (int i = 0; i < ma.Length; i++)
                    sb.AppendLine($"{values[i]:F2}\t{ma[i],6:F2}");
                File.WriteAllText(fileName, sb.ToString());
            }
        }
        #endregion


        #region ---- Build Window Function ----

        const int _maxWindowSize = 100;
        static Dictionary<WindowFunction, decimal[][]> _winFuncs;

        static ICalc()
        {
            _winFuncs = new();
            var len = _maxWindowSize + 1;

            //Identical WF
            _winFuncs[WindowFunction.Identical] = new decimal[len][];
            for (int size = 1; size <= _maxWindowSize; size++)
            {
                _winFuncs[WindowFunction.Identical][size] = Enumerable.Repeat(1m, size).ToArray();
            }

            //Linear WF
            _winFuncs[WindowFunction.Linear] = new decimal[len][];
            for (int size = 1; size <= _maxWindowSize; size++)
            {
                _winFuncs[WindowFunction.Linear][size]
                    = Enumerable.Range(1, size).Select(x => 2m * x / (1 + size)).ToArray();
            }

            _winFuncs[WindowFunction.Gaussian] = new decimal[len][];
            for (int size = 1; size <= _maxWindowSize; size++)
            {
                var func = new decimal[size];
                for (int j = 0; j < size; j++) func[j] = gauss(size, j + 1);
                _winFuncs[WindowFunction.Gaussian][size] = func;
            }
        }

        #endregion


        #region ---- Gaussian Window Function ----

        /// <summary>
        /// Gaussina Window Function: Window Size 에 따른 보정 계수
        /// </summary>
        static double[] _gaussianCoef = new double[_maxWindowSize]
        {
            1.253314E+0,3.120548E+0,5.099726E+0,7.126975E+0,9.177607E+0,
            1.124135E+1,1.331317E+1,1.539033E+1,1.747119E+1,1.955473E+1,
            2.164026E+1,2.372733E+1,2.581559E+1,2.790481E+1,2.999480E+1,
            3.208543E+1,3.417658E+1,3.626818E+1,3.836016E+1,4.045246E+1,
            4.254504E+1,4.463786E+1,4.673090E+1,4.882412E+1,5.091750E+1,
            5.301103E+1,5.510469E+1,5.719846E+1,5.929234E+1,6.138632E+1,
            6.348038E+1,6.557452E+1,6.766873E+1,6.976300E+1,7.185733E+1,
            7.395172E+1,7.604616E+1,7.814065E+1,8.023517E+1,8.232974E+1,
            8.442435E+1,8.651898E+1,8.861366E+1,9.070836E+1,9.280308E+1,
            9.489784E+1,9.699262E+1,9.908742E+1,1.011822E+2,1.032771E+2,
            1.053719E+2,1.074668E+2,1.095617E+2,1.116566E+2,1.137516E+2,
            1.158465E+2,1.179415E+2,1.200364E+2,1.221314E+2,1.242264E+2,
            1.263214E+2,1.284164E+2,1.305115E+2,1.326065E+2,1.347015E+2,
            1.367966E+2,1.388917E+2,1.409867E+2,1.430818E+2,1.451769E+2,
            1.472720E+2,1.493671E+2,1.514622E+2,1.535573E+2,1.556524E+2,
            1.577475E+2,1.598427E+2,1.619378E+2,1.640329E+2,1.661281E+2,
            1.682232E+2,1.703184E+2,1.724135E+2,1.745087E+2,1.766039E+2,
            1.786990E+2,1.807942E+2,1.828894E+2,1.849846E+2,1.870798E+2,
            1.891750E+2,1.912701E+2,1.933653E+2,1.954605E+2,1.975557E+2,
            1.996509E+2,2.017461E+2,2.038414E+2,2.059366E+2,2.080318E+2,
        };

        /// <summary>
        /// μ=s, σ=s/2 인 descrete gaussian distribution
        /// sum[1~s] == s 인 조건을 만족
        /// </summary>
        /// <param name="x"> x=[1..s] </param>
        /// <param name="windowSize"></param>
        /// <returns></returns>
        static decimal gauss(int windowSize, int x)
        {
            var coef = _gaussianCoef[windowSize - 1];
            var exp = coef * Math.Exp(-2.0 * (x - windowSize) / windowSize * (x - windowSize) / windowSize);
            return (decimal)(0.7978845608028654 / windowSize * exp);
        }
        
        #endregion



    }

}
