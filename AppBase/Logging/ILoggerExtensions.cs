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

    /// <summary>
    /// static interface : 모든 event id를 포함/중복검증
    /// </summary>
    public interface ILoggerExtensions
    {
        //key == eventId : id 중복 검증을 위해 dictionary 사용
        protected static Dictionary<int, EventItem> _items = new();

        static void SaveAsJson(string filePath)
        {
            File.WriteAllText(filePath, JS.Serialize(_items.Values,
                new JsonSerializerOptions { WriteIndented = true }));
        }

        protected static void addItem(int id, string name, string template)
        {
            if (_items.ContainsKey(id))
                throw new ArgumentException($"Invalid eventId '{id}': used by {_items[id].name}", nameof(id));
            _items.Add(id, new(id, name, template));
        }

        protected record EventItem(int id, string name, string template)
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
        static Action<ILogger, T1, T2, Exception?> newError<T1, T2>(int id, string name, string template)
        {
            addItem(id, name, template);
            return LoggerMessage.Define<T1, T2>(LogLevel.Error, _items[id].eid, _items[id].template);
        }

        static Func<ILogger, T, IDisposable> newScope<T>(string name)
        {
            return LoggerMessage.DefineScope<T>($"{{{name}}}");
        }


    }//class
}