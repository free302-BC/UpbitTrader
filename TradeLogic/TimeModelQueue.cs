using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;
using Universe.Utility;

namespace Universe.Coin.TradeLogic
{
    public class TimeModelQueue<M> : TimeQueue<M> where M : ICalcModel
    {
        public TimeModelQueue(int duration_sec) : base(duration_sec) { }
        public override void Add(M m) => Add(m, m.Timestamp);

    }
}
