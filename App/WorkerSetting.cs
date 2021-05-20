using IO.Swagger.Client;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit.App
{
    public class WorkerSetting
    {
        public string AccessKey { get; init; } = "";
        public string SecretKey { get; init; } = "";
        public string TokenFile { get; init; } = "";

        //public static implicit operator KeyPair(WorkerSetting set) => (set.AccessKey, set.SecretKey);
        //public static implicit operator WorkerSetting((string AccessKey, string SecretKey) keys)
        //    => new WorkerSetting { AccessKey = keys.AccessKey, SecretKey = keys.SecretKey };

    }//class

    public static class WorkerSettingExtensions
    {
        
    }
}
