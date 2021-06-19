using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic;
using Universe.Utility;
using Xunit;

namespace UnitTester
{
    public class TimeCounterTester
    {
        [Fact]
        static void Test()
        {
            var sb = new StringBuilder();
            var tc = new TimeCounter(1000, 10);
            //tc.sb = sb;
            sb.AppendLine($"{0}\t{tc}");
            var r = new Random();
            for (int i = 0; i < 70; i++)
            {
                tc.Add();
                sb.AppendLine($"{i + 1}\t{tc}");
                Thread.Sleep(100 + r.Next(-50,50));
            }

            Debug.WriteLine(sb.ToString());
            File.WriteAllText("time_counter.txt", sb.ToString());
        }
    }

}
