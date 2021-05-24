﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Universe.AppBase
{
    public static class Extensions
    {
        public static IServiceCollection AddSetting<S>(this ConfigTuple ct, string key) where S : class 
            => ct.Services.Configure<S>(ct.Config.GetSection(key));
        //ct.Services.Configure<S>(s => ct.Config.GetSection(key).Bind(s));//이렇게 하면 change event 발생 안함


        public static IServiceCollection AddSimpleConsole(this IServiceCollection sc)
            => sc.AddLogging(b => b.AddSimpleConsole(opt =>
            {
                opt.IncludeScopes = false;
                opt.TimestampFormat = "[yyMMdd.HHmmss.fff] ";
            }));
        
        public static IServiceCollection AddJsonFile(
            this IServiceCollection sc,
            string jsonFile = "appsettings.json")
            => sc.AddSingleton(
                new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile(jsonFile, false, true)
                .Build());


    }//class
}
