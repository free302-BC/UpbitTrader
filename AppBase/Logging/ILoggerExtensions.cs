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

    public interface ILoggerExtensions
    {
        void SaveAsJson(string filePath);

        protected void addItem(int id, string name, string template);

        protected EventItem this[int id] { get; }

        protected record EventItem(int id, string name, string template)
        {
            [JsonIgnore] public EventId eid = new EventId(id, name);
        }

        Action<ILogger, T, Exception?> newInfo<T>(int id, string name, string template)
        {
            addItem(id, name, template);
            return LoggerMessage.Define<T>(LogLevel.Information, this[id].eid, this[id].template);
        }
        Action<ILogger, T1, T2, Exception?> newInfo<T1, T2>(int id, string name, string template)
        {
            addItem(id, name, template);
            return LoggerMessage.Define<T1, T2>(LogLevel.Information, this[id].eid, this[id].template);
        }
        Action<ILogger, T, Exception?> newError<T>(int id, string name, string template)
        {
            addItem(id, name, template);
            return LoggerMessage.Define<T>(LogLevel.Error, this[id].eid, this[id].template);
        }
        Action<ILogger, T1, T2, Exception?> newError<T1, T2>(int id, string name, string template)
        {
            addItem(id, name, template);
            return LoggerMessage.Define<T1, T2>(LogLevel.Error, this[id].eid, this[id].template);
        }

    }//class
}