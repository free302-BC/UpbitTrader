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
    public class WorkerOptionsBase : IWorkerOptions
    {
        public string AccessKey { get; set; } = "";
        public string SecretKey { get; set; } = "";

        public virtual void Reload(IWorkerOptions source)
        {
            var src = source as WorkerOptionsBase;
            if (src == null)  throw new ArgumentException(
                $"{nameof(WorkerOptionsBase)}.{nameof(Reload)}(): can't reload from type <{source.GetType().Name}>");
            AccessKey = src.AccessKey;
            SecretKey = src.SecretKey;            
        }

        //public static implicit operator KeyPair(WorkerSetting set) => (set.AccessKey, set.SecretKey);
        //public static implicit operator WorkerSetting((string AccessKey, string SecretKey) keys)
        //    => new WorkerSetting { AccessKey = keys.AccessKey, SecretKey = keys.SecretKey };

    }//class    
}
