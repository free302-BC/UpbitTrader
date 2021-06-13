using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Universe.Coin.TradeLogic.Model
{
    /// <summary>
    /// json serializer for interface types
    /// </summary>
    /// <typeparam name="I">interface</typeparam>
    public class JcInterface<I> : JsonConverter<I>// where I : class
    {
        readonly Type _implType;
        public JcInterface(Type implType) => _implType = implType;

        public override I? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = reader.GetString() ?? throw new Exception();
            var model = JsonSerializer.Deserialize(json, _implType, options);// ?? default(I);
            return (I?)model;
        }
        public override void Write(Utf8JsonWriter writer, I value, JsonSerializerOptions options)
        {
            var json = JsonSerializer.Serialize(value, options);
            writer.WriteStringValue(json);
        }
    }
}
