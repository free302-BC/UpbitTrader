using IO.Swagger.Client;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WC = System.Net.WebClient;

namespace Universe.Coin.Upbit.App
{
    public class WorkerSetting
    {
        public string AccessKey { get; init; } = "";
        public string SecretKey { get; init; } = "";

        public static implicit operator (string AccessKey, string SecretKey)(WorkerSetting set) => (set.AccessKey, set.SecretKey);
        public static implicit operator WorkerSetting((string AccessKey, string SecretKey) keys)
            => new WorkerSetting { AccessKey = keys.AccessKey, SecretKey = keys.SecretKey };

    }//class

    public static class WorkerSettingExtensions
    {
        public static void BuildAuthToken(this WC wc, WorkerSetting set)
            => wc.Headers.Add("Authorization", (set.AccessKey, set.SecretKey).BuildAuthToken());

        public static void BuildAuthToken(this WC wc, WorkerSetting set, NameValueCollection nvc)
            => wc.Headers.Add("Authorization", (set.AccessKey, set.SecretKey).BuildAuthToken(nvc));
    }
}
