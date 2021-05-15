using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Universe.Coin.Upbit.App
{
    public static class Extensions
    {
        public static IServiceCollection AddJsonFile(this IServiceCollection sc, out IConfiguration config,
            string jsonFile = "appsettings.json")
            => sc.AddSingleton(
                config = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory).AddJsonFile(jsonFile).Build());

        public static IServiceCollection AddSetting<S>(this ConfigTuple ct, string key) where S : class
            => ct.Services.Configure<S>(s => ct.Config.GetSection(key).Bind(s));

        public static IServiceCollection AddSimpleConsole(this IServiceCollection sc)
            => sc.AddLogging(b => b.AddSimpleConsole(opt =>
            {
                opt.IncludeScopes = false;
                opt.TimestampFormat = "[yyMMdd.HHmmss.fff] ";
            }));

        public static string ToText<T>(this IEnumerable<T> list)
        {
            var sb = new StringBuilder();
            foreach (var i in list) sb.AppendLine($"{i}");
            return sb.ToString();
        }

    }//class
}
