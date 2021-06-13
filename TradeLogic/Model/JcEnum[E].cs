using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Universe.Utility;

namespace Universe.Coin.TradeLogic.Model
{
    /// <summary>
    /// string을 enum 'E'로 변환 - enum name과 string이 다른 경우
    /// </summary>
    /// <typeparam name="C">converter class</typeparam>
    /// <typeparam name="E">enum</typeparam>
    public abstract class JcEnum<C, E> : JsonConverter<E> where C: JcEnum<C, E> where E : struct
    {
        static SortedBiDictionary<string, E> _dic;

        static JcEnum()
        {
            _dic = new();
        }
        protected static void init(string[] names)
        {
            var values = (E[])Enum.GetValues(typeof(E));
            for (int i = 0; i < values.Length; i++) _dic.Add(names[i], values[i]);
        }

        public override E Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = reader.GetString()!;
            var model = _dic[json];
            return model;
        }

        public override void Write(Utf8JsonWriter writer, E value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(_dic[value]);
        }

    }//class
}
