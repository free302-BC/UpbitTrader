using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Universe.AppBase.Logging
{
    /// <summary>
    /// 일반적인 메시지 로그 메소드 정의
    /// </summary>
    public static class GeneralLoggerExtensions
    {
        static ILoggerExtensions _impl = new LoggerExtensionsImpl();
        static GeneralLoggerExtensions()
        {
            _impl.SaveAsJson("logging_events.json");
        }

        // Info(object? message)
        static readonly Action<ILogger, object?, Exception?> _Info
            = _impl.newInfo<object?>(1, nameof(Info), "{message}");
        public static void Info(this ILogger logger, object? message)
            => _Info(logger, message, null);


        // Info2(object? message1, object? message2)
        static readonly Action<ILogger, object?, object?, Exception?> _Info2
            = _impl.newInfo<object?, object?>(2, nameof(Info),
                $"{{message1}}{Environment.NewLine}{{message2}}");
        public static void Info(this ILogger logger, object? message1, object? message2)
            => _Info2(logger, message1, message2, null);


        // Error(object? message, Exception exception)
        static readonly Action<ILogger, object?, Exception?> _Error
            = _impl.newError<object?>(-1, nameof(Error), "{message}");
        public static void Error(this ILogger logger, object? message, Exception? exception = default)
            => _Error(logger, message, exception);


        // Error2(object? msg1, object? msg2)
        static readonly Action<ILogger, object?, object?, Exception?> _Error2
            = _impl.newError<object?, object?>(-2, nameof(Error), "{msg1}, {msg2}");
        public static void Error(this ILogger logger, object? msg1, object? msg2)
              => _Error2(logger, msg1, msg2, null);



    }//class




}