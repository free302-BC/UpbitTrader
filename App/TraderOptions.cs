using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.AppBase;

namespace Universe.Coin.Upbit.App
{
    public class TraderOptions : WorkerOptionsBase
    {
        public override void Reload(IWorkerOptions source)
        {
            base.Reload(source);
            var src = source as TraderOptions;
            if (src == null) throw new ArgumentException(
               $"{nameof(TraderOptions)}.{nameof(Reload)}(): can't reload from type <{source.GetType().Name}>");
        }
    }
}
