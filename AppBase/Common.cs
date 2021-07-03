using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Universe.AppBase
{
    using JS = JsonSerializer;
    using JSO = JsonSerializerOptions;

    public static class Extensions
    {
        public static JSO Init(this JSO? opt)
        {
            opt = opt ?? new JSO();
            opt.IncludeFields = true;
            opt.WriteIndented = true;
            opt.PropertyNameCaseInsensitive = false;
            opt.NumberHandling = JsonNumberHandling.AllowReadingFromString
                | JsonNumberHandling.AllowNamedFloatingPointLiterals;
            opt.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            opt.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.HangulSyllables);
            return opt;
        }

    }


}
