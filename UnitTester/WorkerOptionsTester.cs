using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.Upbit.App;
using Xunit;

namespace UnitTester
{
    public class WorkerOptionsTester
    {
        [Fact] void reload_clone_WorkerOptions()
        {
            WorkerOptions w = new();
            w.AccessKey = "ak:xyz";
            w.SecretKey = "sk:stu";

            WorkerOptions w2 = new();
            w2.Reload(w);
            Assert.Equal(w2.AccessKey, w.AccessKey);
            Assert.Equal(w2.SecretKey, w.SecretKey);

            var w3 = (WorkerOptions)w.Clone();
            Assert.Equal(w3.AccessKey, w.AccessKey);
            Assert.Equal(w3.SecretKey, w.SecretKey);
        }

        [Fact]
        void reload_clone_BackTestOptions()
        {
            BackTestOptions w = new();
            w.AccessKey = "ak:xyz";
            w.DoFindK = true;
            w.CalcParam.FactorK = 123.456m;

            BackTestOptions w2 = new();
            w2.Reload(w);
            check(w, w2);

            var w3 = (BackTestOptions)w.Clone();
            check(w, w3);

            static void check(BackTestOptions exp, BackTestOptions act)
            {
                Assert.Equal(exp.AccessKey, act.AccessKey);
                Assert.Equal(exp.DoFindK, act.DoFindK);
                Assert.Equal(exp.CalcParam.FactorK, act.CalcParam.FactorK);
            }
        }

        [Fact]
        void reload_clone_TraderOptions()
        {
            TraderOptions w = new();
            w.AccessKey = "ak:xyz";
            w.Pausing = true;
            w.CalcParam.FactorK = 123.456m;

            TraderOptions w2 = new();
            w2.Reload(w);
            check(w, w2);

            var w3 = (TraderOptions)w.Clone();
            check(w, w3);

            static void check(TraderOptions exp, TraderOptions act)
            {
                Assert.Equal(exp.AccessKey, act.AccessKey);
                Assert.Equal(exp.Pausing, act.Pausing);
                Assert.Equal(exp.CalcParam.FactorK, act.CalcParam.FactorK);
            }
        }
    }
}
