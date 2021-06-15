using System;
using System.Text.Json.Serialization;

namespace Universe.Coin.TradeLogic.Model
{
    //[JsonInterfaceConverter(typeof(ThingConverter))]
    public interface IWsResponse : IApiModel
    {
        string Market { get; set; }
        string StreamType { get; set; }
        TradeEvent Event { get; set; }
    }

    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class JsonInterfaceConverterAttribute : JsonConverterAttribute
    {
        public JsonInterfaceConverterAttribute(Type converterType) : base(converterType)
        {
        }
    }

}