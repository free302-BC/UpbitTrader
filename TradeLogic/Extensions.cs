using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic
{
    using JSO = JsonSerializerOptions;

    public static class Extensions
    {
        public static string Print(this IEnumerable<IViewModel> models) => IViewModel.Print(models);
        public static string Print(this IEnumerable<IViewModel> models, int offset, int count)
            => IViewModel.Print(models, offset, count);

        public static void LogWebException(this ILogger logger, HttpRequestException ex)
        {
            var nl = Environment.NewLine;
            var msg = $"ex.StatusCode={ex.StatusCode}{nl}ex.Message= {ex.Message}";

            //if (ex. != null)
            //{
            //    var res = (HttpWebResponse)ex.Response;
            //    Span<byte> buffer = stackalloc byte[1024];
            //    res.GetResponseStream().Read(buffer);
            //    var text = Encoding.ASCII.GetString(buffer);
            //    msg = $"{msg}{nl}res.StatusCode= {res.StatusCode}{nl}res.Content= {text}";
            //}
            logger.LogError(msg);
        }

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
