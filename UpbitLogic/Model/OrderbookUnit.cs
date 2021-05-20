﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit.Model
{
    [DataContract]
    public class OrderbookUnit
    {
        
        /// <summary>
        /// 매도호가
        /// </summary>
        /// <value>매도호가</value>
        [DataMember(Name = "ask_price", EmitDefaultValue = false)]
        public double AskPrice { get; set; }

        /// <summary>
        /// 매수호가
        /// </summary>
        /// <value>매수호가</value>
        [DataMember(Name = "bid_price", EmitDefaultValue = false)]
        public double BidPrice { get; set; }

        /// <summary>
        /// 매도 잔량
        /// </summary>
        /// <value>매도 잔량</value>
        [DataMember(Name = "ask_size", EmitDefaultValue = false)]
        public double AskSize { get; set; }

        /// <summary>
        /// 매수 잔량
        /// </summary>
        /// <value>매수 잔량</value>
        [DataMember(Name = "bid_size", EmitDefaultValue = false)]
        public double BidSize { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"AskPrice: {AskPrice,10}");
            sb.Append($"BidPrice: {BidPrice,10}");
            sb.Append($"AskSize:  {AskSize,10}");
            sb.Append($"BidSize:  {BidSize,10}");
            return sb.ToString();
        }

    }
}