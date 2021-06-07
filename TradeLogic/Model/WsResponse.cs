using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic.Model
{
    
#pragma warning disable CS8618

    public class WsResponse : IApiModel
    {
        //public WsResponse()
        //{
        //    requestType = requestMarket = requestStreamType = "";
        //}

        [JsonPropertyName("type")]
        public string requestType;// { get; set; }
        [JsonPropertyName("code")]
        public string requestMarket;// { get; set; }
        [JsonPropertyName("stream_type")]
        public string requestStreamType;// { get; set; }

    }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

}
