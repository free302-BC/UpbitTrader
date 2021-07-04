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
    using JS = JsonSerializer;

    public static class ILoggerExtensions
    {
        #region ---- Item Dic & Methods ----

        //key == eventId : id 중복 검증을 위해 dictionary 사용
        static Dictionary<int, EventItem> _items = new();
        static void addItem(int id, string name, string template)
        {
            if (_items.ContainsKey(id))
                throw new ArgumentException($"Invalid eventId '{id}': used by {_items[id].name}", nameof(id));
            _items.Add(id, new(id, name, template));
        }

        record EventItem(int id, string name, string template)
        {
            [JsonIgnore] public EventId eid = new EventId(id, name);
        }

        static Action<ILogger, T, Exception?> newInfo<T>(int id, string name, string template)
        {
            addItem(id, name, template);
            return LoggerMessage.Define<T>(LogLevel.Information, _items[id].eid, _items[id].template);
        }
        static Action<ILogger, T1, T2, Exception?> newInfo<T1, T2>(int id, string name, string template)
        {
            addItem(id, name, template);
            return LoggerMessage.Define<T1, T2>(LogLevel.Information, _items[id].eid, _items[id].template);
        }
        static Action<ILogger, T, Exception?> newError<T>(int id, string name, string template)
        {
            addItem(id, name, template);
            return LoggerMessage.Define<T>(LogLevel.Error, _items[id].eid, _items[id].template);
        }

        static ILoggerExtensions()
        {
            File.WriteAllText("logging_events.json", JS.Serialize(_items.Values,
                new JsonSerializerOptions { WriteIndented = true }));
        }

        #endregion

        // Info(object? message)
        static readonly Action<ILogger, object?, Exception?> _Info 
            = newInfo<object?>(1, nameof(Info), "{message}");
        public static void Info(this ILogger logger, object? message)
            => _Info(logger, message, null);

        // Info2(object? message1, object? message2)
        static readonly Action<ILogger, object?, object?, Exception?> _Info2 
            = newInfo<object?, object?>(2, nameof(Info),
                $"{{message1}}{Environment.NewLine}{{message2}}");
        public static void Info(this ILogger logger, object? message1, object? message2)
            => _Info2(logger, message1, message2, null);

        // Error(object? message, Exception exception)
        static readonly Action<ILogger, object?, Exception?> _Error 
            = newError<object?>(-1, nameof(Error), "{message}");
        public static void Error(this ILogger logger, object? message, Exception? exception = default)
            => _Error(logger, message, exception);




    }//class
}