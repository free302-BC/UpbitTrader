using System;
using System.Collections.Generic;
using System.Threading;
using Universe.Utility;

namespace Universe.Coin.App
{
    using SignalDic = SortedMultiDictionary<ConsoleKey, ConsoleModifiers, List<EventWaitHandle>>;

    public interface IInputProvider
    {
        protected abstract SignalDic _signals { get; }
        protected object _lock { get; }

        public void AddSignal(ConsoleKey key, EventWaitHandle signal, ConsoleModifiers mod = 0)
        {
            lock (_lock)
            {
                if (_signals.ContainsKey(key, mod)) _signals[key, mod].Add(signal);
                else _signals[key, mod] = new() { signal };
            }
        }
        public void RemoveSignal(ConsoleKey key, EventWaitHandle signal, ConsoleModifiers mod = 0)
        {
            lock (_lock)
            {
                if (_signals.ContainsKey(key, mod))
                {
                    var result = _signals[key, mod].Remove(signal);
                    if (result && _signals[key, mod].Count == 0) _signals.Remove(key, mod);
                }
            }
        }


    }
}