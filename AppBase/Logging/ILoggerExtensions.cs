using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Universe.AppBase.Logging
{
    public static class ILoggerExtensions
    {
        // Test(string name)
        static readonly Action<ILogger, string, Exception?> _Test
        = LoggerMessage.Define<string>(LogLevel.Information, new EventId(1, nameof(Test)), 
            "name = '{name}' ");
        public static void Test(this ILogger logger, string name2)
            => _Test(logger, name2, null);

        

    }//class
}