using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Universe.AppBase;

namespace Universe.AppBase
{
    public class WorkerOptionsBase : IWorkerOptions
    {
        public virtual void Reload(IWorkerOptions source)
        {
            var src = source as WorkerOptionsBase;
            if (src == null)  throw new ArgumentException(
                $"{nameof(WorkerOptionsBase)}.{nameof(Reload)}(): can't reload from type <{source.GetType().Name}>");
        }

        //public static implicit operator KeyPair(WorkerSetting set) => (set.AccessKey, set.SecretKey);
        //public static implicit operator WorkerSetting((string AccessKey, string SecretKey) keys)
        //    => new WorkerSetting { AccessKey = keys.AccessKey, SecretKey = keys.SecretKey };

    }//class    
}
