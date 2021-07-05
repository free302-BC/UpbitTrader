using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Universe.AppBase.Logging
{
    using JS = JsonSerializer;

    public class LoggerExtensionsImpl : ILoggerExtensions
    {
        //key == eventId : id 중복 검증을 위해 dictionary 사용
        Dictionary<int, ILoggerExtensions.EventItem> _items = new();

        public void SaveAsJson(string filePath)
        {
            File.WriteAllText(filePath, JS.Serialize(_items.Values,
                new JsonSerializerOptions { WriteIndented = true }));
        }

        ILoggerExtensions.EventItem ILoggerExtensions.this[int id] => _items[id];

        void ILoggerExtensions.addItem(int id, string name, string template)
        {
            if (_items.ContainsKey(id))
                throw new ArgumentException($"Invalid eventId '{id}': used by {_items[id].name}", nameof(id));
            _items.Add(id, new(id, name, template));
        }

    }//class
}
