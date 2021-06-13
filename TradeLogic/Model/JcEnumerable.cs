using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic.Model
{
    public class JcEnumerable<C> : JcEnumerable<C, C> where C : class
    {
    }

    /// <summary>
    /// 인터페이스 I의 IEnumerable에 대한 Json Converter
    /// </summary>
    /// <typeparam name="I">인터페이스</typeparam>
    /// <typeparam name="C">구현 클래스</typeparam>
    public class JcEnumerable<I, C> : JsonConverter<IEnumerable<I>> where C : class, I
    {
        public override IEnumerable<I> Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var text = doc.RootElement.GetRawText();
            var models = JsonSerializer.Deserialize<C[]>(text, options) ?? Array.Empty<I>();
            return models;
        }

        public override void Write(Utf8JsonWriter writer, IEnumerable<I> value, JsonSerializerOptions options)
        {
            var ser = JsonSerializer.Serialize(value, options);
            writer.WriteStringValue(ser);
        }

    }
}
