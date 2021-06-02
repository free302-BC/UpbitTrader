using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic
{
    /// <summary>
    /// 주어진 시간동안 카운터의 갯수를 주어진 한계로 유지 
    /// </summary>
    public class TimeCounter
    {
        public TimeCounter(int duration_ms, int maxCount)
        {
            Duration = duration_ms;
            MaxCount = maxCount;
            len = 2 * maxCount;
            buffer = new long[len];

            watch = Stopwatch.StartNew();
            buffer[0] = watch.ElapsedMilliseconds;
            i0 = i1 = 0;
            count = 1;
        }

        public readonly int Duration;
        public readonly int MaxCount;

        int len;
        int i0, i1;//index of first, last time
        readonly long[] buffer;
        readonly Stopwatch watch;

        int count;// => 1 + (10 + i1 - i0) % MaxCount;
        long dt => buffer[i1] - buffer[i0];

        /// <summary>
        /// 카운트 증가하고 필요시 대기 후 리턴
        /// </summary>
        public void Add()
        {
            //카운트가 찼을 경우 대기
            if (count >= MaxCount && dt < Duration) Thread.Sleep(Duration - (int)dt);

            //카운트 증가, 시간 기록
            buffer[i1 = (i1 + 1) % len] = watch.ElapsedMilliseconds;
            count++;

            //오래된 데이터 삭제
            while (dt > Duration)
            {
                i0 = (i0 + 1) % len;
                count--;
            }
        }
        public override string ToString() => $"{i0}\t{i1}\t{count}\t{dt}";
        public void Dump(StringBuilder sb)
        {
            sb.AppendLine(ToString());
            for (int i = 0; i < len; i++) sb.AppendLine($"{i,2}\t{buffer[i]}");
        }

    }//class
}
