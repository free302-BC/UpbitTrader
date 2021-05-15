using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Universe.Coin.Upbit.App
{
    public class TestWorker : WorkerBase<TestWorker, WorkerSetting>
    {
        public TestWorker(ILogger<TestWorker> logger, IOptionsMonitor<WorkerSetting> set) : base(logger, set) { }

        protected override void work(WorkerSetting set)
        {
            var config = Configuration.Default;
            var token = Helper.buildAuthToken(set.AccessKey, set.SecretKey);
            config.ApiKey.Add("Authorization", token);
            config.ApiKeyPrefix.Add("Authorization", "Bearer");

            var apiInstance = new APIKeyApi();
            try
            {
                // API 키 리스트 조회
                List<APIKey> result = apiInstance.APIKeyInfo();
                info($"------- {result.ToText()} ------------");
            }
            catch (Exception e)
            {
                log("Exception when calling APIKeyApi.APIKeyInfo:\n" + e.Message);
            }
        }

    }//class
}
