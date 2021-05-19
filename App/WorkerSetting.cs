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

        //public static implicit operator (string AccessKey, string SecretKey)(WorkerSetting set) => (set.AccessKey, set.SecretKey);
        //public static implicit operator WorkerSetting((string AccessKey, string SecretKey) keys)
        //    => new WorkerSetting { AccessKey = keys.AccessKey, SecretKey = keys.SecretKey };

    }//class

    public static class WorkerSettingExtensions
    {
        public static string CheckAuthKey(this WorkerSetting set)
        {
            if (File.Exists(set.TokenFile))
            {
                //key를 사용자에게 입력 받는 방법 필요 - 다중 사용자 web
                Helper.SaveAuthToken(set.AccessKey, set.SecretKey, set.TokenFile);
            }
            return Helper.LoadAuthToken(set.TokenFile);
        }
    }
}
