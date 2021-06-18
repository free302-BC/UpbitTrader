using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic;
using Xunit;

namespace UnitTester
{
    public class TimingSignalTester
    {
        [Fact]
        void logic_table()
        {
            var models = ((TimingSignal[])Enum.GetValues(typeof(TimingSignal))).Select(x => (M)x).ToArray();
            var signals = new TimingSignal[] { TimingSignal.None, TimingSignal.DoBuy, TimingSignal.DoSell };

            foreach (var prev in models)
                foreach (var signal in signals)
                {
                    var r = new R(prev, signal);
                    Debug.WriteLine(r);
                    Assert.Equal(r.F2, r.F1);
                }
        }
        static TimingSignal f1(M prev, TimingSignal signal)
        {
            return prev.Signal switch
            {
                TimingSignal.None or TimingSignal.DoSell
                    => signal == TimingSignal.DoBuy ? signal : TimingSignal.None,
                _ => signal == TimingSignal.DoSell ? signal : TimingSignal.Hold,
            };
        }
        static TimingSignal f2(M prev, TimingSignal signal)
        {
            M model = TimingSignal.None;
            if (prev.Signal == TimingSignal.None)
            {
                if (signal == TimingSignal.DoBuy) model = signal;//dobuy
                else model = TimingSignal.None;
            }
            if (prev.Signal == TimingSignal.DoBuy)
            {
                if (signal != TimingSignal.DoSell) model = TimingSignal.Hold;
                else model = signal;//dosell
            }
            if (prev.Signal == TimingSignal.Hold)
            {
                if (signal != TimingSignal.DoSell) model = TimingSignal.Hold;
                else model = signal;//dosell
            }
            if (prev.Signal == TimingSignal.DoSell)
            {
                if (signal == TimingSignal.DoBuy) model = signal;//dobuy
                else model = TimingSignal.None;
            }
            return model.Signal;
        }
        record M(TimingSignal Signal)
        {
            public static implicit operator M(TimingSignal signal) => new M(signal);
            public static implicit operator TimingSignal(M m) => m.Signal;
        }
        record R(M prev, TimingSignal signal)
        {
            public TimingSignal F1 => f1(prev, signal);
            public TimingSignal F2 => f2(prev, signal);
        }

    }
}
