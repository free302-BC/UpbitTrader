using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic;
using Universe.Coin.TradeLogic.Model;
using Xunit;

namespace UnitTester
{
    public class TimeQueueTester
    {
        static Stopwatch _watch = Stopwatch.StartNew();

        [Fact]public static void timeQ_calcModel()
        {
            var data = Enumerable.Range(0, 100000).Select(x => new M(x * 100));
            var q1 = new TimeModelQueue<M>(600);
            var q2 = new TimeModelQueue2<M>(600);

            timing(q1, data, "q1");//7.8sec
            timing(q2, data, "q2");//9.6
            timing(q1, data, "q1");//7.8
            timing(q2, data, "q2");//9.9

            void timing(ITimeQueue<M> q, IEnumerable<M> data, string name)
            {
                q.Clear();
                _watch.Restart();
                foreach (var m in data)
                {
                    q.Add(m);
                    //Debug.WriteLine(q);
                    //Thread.Sleep(100);
                }
                Debug.WriteLine($"{name}: {_watch.ElapsedMilliseconds}ms");
                Console.WriteLine($"{name}: {_watch.ElapsedMilliseconds}ms");
            }

        }
        record M : ICalcModel
        {
            public M(long ts) => Timestamp = ts;
            public long Timestamp { get; set; }
            public decimal MovingAvg { get; set; }
            public decimal MacdOsc { get; set; }
            public decimal Target { get; set; }
            public TimingSignal Signal { get; set; }
            public bool TradeDone { get; set; }
            public decimal Rate { get; set; }
            public decimal CumRate { get; set; }
            public decimal DrawDown { get; set; }

            public string CalcHeader => throw new NotImplementedException();

            public string ToCalcString()
            {
                throw new NotImplementedException();
            }
        }


        [Fact] void generic_timeQ()
        {
            Random r = new();
            var q = new TimeQueue<int>(5);
            for (int i = 0; i < 100; i++)
            {
                q.Add(i);
                Debug.WriteLine(q);
                Thread.Sleep(100 + r.Next(-100, 100));
            }
            var arr = q.ToArray();
        }


        [Fact] void test()
        {
            var now = DateTime.Parse(DateTime.Now.ToString());
            var local = now.ToLocalTime().ToUniversalTime();
            var utc = now.ToUniversalTime().ToLocalTime();
        }

    }

    public class TimeModelQueue2<M> : ITimeQueue<M> where M : ICalcModel
    {
        readonly int _duration;//아이템을 유지할 시간
        readonly Queue<M> _q;//
        public TimeModelQueue2(int duration_sec)
        {
            _duration = duration_sec * 1000;
            _q = new();
        }
        public override string ToString() => $"dt={dt}, count={_q.Count}";
        public string ToTimeString()
        {
            var times = _q.Select(x => x.Timestamp);
            return $"dt={dt}, count={_q.Count}: [{string.Join(' ', times)}]";
        }

        long dt => _q.Last().Timestamp - _q.First().Timestamp;
        public void Add(M model)
        {
            _q.Enqueue(model);
            while (dt > _duration) _q.Dequeue();
        }
        public void Clear() => _q.Clear();
        public M[] ToArray()
        {
            if (_q.Any()) while (dt > _duration) _q.Dequeue();
            return _q.ToArray();
        }
    }//class

}
