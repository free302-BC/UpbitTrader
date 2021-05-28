using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Analysis;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic
{
    public class BuyOverDelta : ITradeLogic
    {
        public void CalcProfitRate(IList<CandleModel> models, decimal k)
        {
            CalcProfitRate(models, k, 0);
        }
        public void CalcProfitRate(IList<CandleModel> models, decimal k, decimal ma = 0)
        {
            CalcProfitRate(models[0], CandleModel._empty, k);
            for (int i = 1; i < models.Count; i++)
                CalcProfitRate(models[i], models[i - 1], k);
        }
        public void CalcProfitRate(CandleModel model, CandleModel prev, decimal k, decimal ma = 0)
        {
            model.Target = Math.Round(model.Opening + prev.Delta * k, 2);
            model.Rate = (model.High > model.Target) ? Math.Round(model.Closing / model.Target - CandleModel.FeeRate, 4) : 1.0m;
        }

        public static void TestDataFrame(Action<object> info)
        {
            PrimitiveDataFrameColumn<DateTime> dateTimes = new("DateTimes"); // Default length is 0.
            PrimitiveDataFrameColumn<int> ints = new("Ints", 3); // Makes a column of length 3. Filled with nulls initially
            StringDataFrameColumn strings = new("Strings", 3); // Makes a column of length 3. Filled with nulls initially

            // Append 3 values to dateTimes
            dateTimes.Append(DateTime.Parse("2019/01/01"));
            dateTimes.Append(DateTime.Parse("2019/01/01"));
            dateTimes.Append(DateTime.Parse("2019/01/02"));

            DataFrame df = new(dateTimes, ints, strings); // This will throw if the columns are of different lengths

            df[0, 1] = 10;
            ints[1] = 100;
            strings[1] = "Foo";

            ints.Add(5, true);
            info("ints=", ints);

            df.Columns["Ints"] = (ints / 5) * 100;
            info(ints);
            info(df);
            

            info(df.Info());
        }
        
    }

    public class BuyOverDeltaMA : ITradeLogic
    {
        public void CalcProfitRate(IList<CandleModel> models, decimal k)
        {
            for (int i = 1; i < models.Count; i++)
                CalcProfitRate(models[i], models[i - 1], k);
        }
        public void CalcProfitRate(IList<CandleModel> models, decimal k, decimal ma = 0)
        {
            CalcProfitRate(models[0], CandleModel._empty, k);
            for (int i = 1; i < models.Count; i++)
                CalcProfitRate(models[i], models[i - 1], k);
        }
        public void CalcProfitRate(CandleModel model, CandleModel prev, decimal k, decimal ma = 0)
        {
            model.Target = Math.Round(model.Opening + prev.Delta * k, 2);
            model.Rate = (model.High > model.Target && model.Opening >= ma)
                ? Math.Round(model.Closing / model.Target - CandleModel.FeeRate, 4)
                : 1.0m;
        }
    }

}
