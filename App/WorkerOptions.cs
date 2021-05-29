using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Universe.AppBase;

namespace Universe.Coin.Upbit.App
{
    public class WorkerOptions : WorkerOptionsBase
    {
        public string AccessKey { get; set; } = "";
        public string SecretKey { get; set; } = "";

        public override void Reload(IWorkerOptions source)
        {
            base.Reload(source);
            var src = source as WorkerOptions;
            if (src == null)  throw new ArgumentException(
                $"{nameof(WorkerOptions)}.{nameof(Reload)}(): can't reload from type <{source.GetType().Name}>");
            AccessKey = src.AccessKey;
            SecretKey = src.SecretKey;            
        }

        //public static implicit operator KeyPair(WorkerSetting set) => (set.AccessKey, set.SecretKey);
        //public static implicit operator WorkerSetting((string AccessKey, string SecretKey) keys)
        //    => new WorkerSetting { AccessKey = keys.AccessKey, SecretKey = keys.SecretKey };

    }//class    
}
