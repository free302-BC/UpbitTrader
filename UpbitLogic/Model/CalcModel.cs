using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit.Model
{
    public class CalcModel
    {
        public double Opening;
        public double High;
        public double Low;
        public double Closing;

        public double Rate { get; set; }

        public double NextTarget;

    }//class
}
