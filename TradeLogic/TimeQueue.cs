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

namespace Universe.Utility
{
    public interface ITimeQueue<M>
    {
        void Add(M model);
        void Clear();
        M[] Last(int count);
        M[] ToArray();
        string ToString();
        string ToTimeString();
    }
    
    /// <summary>
    /// 지정한 시간동안 아이템을 유지하는 큐
    /// TODO: thread-safe?
    /// </summary>
    public class TimeQueue<M> : ITimeQueue<M>
    {
        readonly int _duration;//아이템을 유지할 시간
        readonly Stopwatch _watch;
        readonly Queue<M> _mq;//
        readonly Queue<long> _tq;//time queue
        public TimeQueue(int duration_sec)
        {
            _duration = duration_sec * 1000;
            _watch = Stopwatch.StartNew();
            _mq = new();
            _tq = new();
        }
        public override string ToString() => $"dt={dt}, count={_mq.Count}";
        public string ToTimeString() => $"dt={dt}, count={_tq.Count}: [{string.Join(' ', _tq)}]";

        long dt => _tq.Last() - _tq.First();
        public virtual void Add(M m) => Add(m, _watch.ElapsedMilliseconds);
        public void Add(M m, long ms)
        {
            _mq.Enqueue(m);
            _tq.Enqueue(ms);
            dequeue();
        }
        public void Clear()
        {
            _mq.Clear();
            _tq.Clear();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void dequeue()
        {
            while (dt > _duration)
            {
                _mq.Dequeue();
                _tq.Dequeue();
            }
        }
        public M[] ToArray()
        {
            if (_tq.Any()) dequeue();                
            return _mq.ToArray();
        }

        public M[] Last(int count)
        {
            return _mq.TakeLast(count).ToArray();
        }


    }//class
}
