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
            config.ApiKeyPrefix.Add("Authorization", "Bearer");

            var keyApi = new APIKeyApi();
            var accountApi = new AccountApi();
            try
            {
                // API 키 리스트 조회
                config.ApiKey["Authorization"] = (set.AccessKey, set.SecretKey).BuildAuthToken();
                List<APIKey> result = keyApi.APIKeyInfo();
                info($"\n---------------\n{result.ToText()}\n---------------");

                var account = accountApi.AccountInfo();
                info($"\n---------------\n{account.ToText()}\n---------------");
            }
            catch (Exception e)
            {
                log("Exception when calling APIKeyApi.APIKeyInfo:\n" + e.Message);
            }
        }

    }//class
}
