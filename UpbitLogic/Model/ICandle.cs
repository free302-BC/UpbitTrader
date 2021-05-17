using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit.Model
{
    public interface ICandle
    {
        /// <summary>
        /// 시가
        /// </summary>
        /// <value>시가</value>
        [DataMember(Name = "opening_price", EmitDefaultValue = false)]
        public double OpeningPrice { get; set; }

        /// <summary>
        /// 고가
        /// </summary>
        /// <value>고가</value>
        [DataMember(Name = "high_price", EmitDefaultValue = false)]
        public double HighPrice { get; set; }

        /// <summary>
        /// 저가
        /// </summary>
        /// <value>저가</value>
        [DataMember(Name = "low_price", EmitDefaultValue = false)]
        public double LowPrice { get; set; }

        /// <summary>
        /// 종가
        /// </summary>
        /// <value>종가</value>
        [DataMember(Name = "trade_price", EmitDefaultValue = false)]
        public double TradePrice { get; set; }

    }
}
