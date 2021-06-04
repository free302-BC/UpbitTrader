using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic.Calc
{
    public struct MacdParam
    {
        public int Fast { get; set; }
        public int Slow { get; set; }
        public int Signal { get; set; }

        public MacdParam(MacdParam param)
        {
            Fast = param.Fast;
            Slow = param.Slow;
            Signal = param.Signal;
        }
        public MacdParam(int fastSize, int slowSize, int signalSize)
        {
            Fast = fastSize;
            Slow = slowSize;
            Signal = signalSize;
        }
        public override string ToString() => $"({Fast}, {Slow}, {Signal})";

        public static implicit operator int[](MacdParam param)
            => new[] { param.Fast, param.Slow, param.Signal };
        public static implicit operator MacdParam(int[] array)
            => new(array[0], array[1], array[2]);

    }//struct
}
