using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Universe.AppBase;
using Universe.Coin.TradeLogic;

namespace Universe.Coin.Upbit.App
{
    public class WorkerOptions : TradeOptionsBase, IWorkerOptions
    {
        public void Reload(IWorkerOptions source)
        {
            IWorkerOptions.checkType(this, source);
            var src = (WorkerOptions)source;
            AccessKey = src.AccessKey;
            SecretKey = src.SecretKey;
        }

        public IWorkerOptions Clone()
        {
            var dest = new WorkerOptions();
            dest.AccessKey = AccessKey;
            dest.SecretKey = SecretKey;
            return dest;
        }

    }//class    
}
