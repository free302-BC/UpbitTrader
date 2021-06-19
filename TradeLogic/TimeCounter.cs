using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Universe.Utility
{
    /// <summary>
    /// 주어진 시간동안 카운터의 갯수를 주어진 한계로 유지 
    /// </summary>
    public class TimeCounter
    {
        readonly int _duration;//아이템을 유지할 시간
        readonly int _capacity;//유지할 아이템의 최대갯수
        readonly Stopwatch _watch;
        public TimeCounter(int duration_ms, int maxCount)
        {
            _duration = duration_ms;
            _capacity = maxCount;
            len = 2 * maxCount;
            buffer = new long[len];

            _watch = Stopwatch.StartNew();
            buffer[0] = _watch.ElapsedMilliseconds;
            i0 = i1 = 0;
            count = 1;
        }

        readonly long[] buffer;//내부버퍼
        readonly int len;//내부버퍼 길이 = 2 * _maxCount

        int i0, i1;//유효아이템의 시작, 끝 인덱스
        int count;//현재 유효 아이템 갯수
        long dt => buffer[i1] - buffer[i0];//첫 아이템과 끝 아이템의 시각차

        /// <summary>
        /// 카운트 증가하고 필요시 대기 후 리턴
        /// </summary>
        public void Add()
        {
            //카운트가 찼을 경우 대기
            if (count >= _capacity && dt < _duration) Thread.Sleep(_duration - (int)dt);

            //카운트 증가, 시간 기록
            buffer[i1 = (i1 + 1) % len] = _watch.ElapsedMilliseconds;
            count++;

            //오래된 데이터 삭제
            while (dt > _duration)
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
