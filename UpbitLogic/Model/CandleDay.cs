using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic;

namespace Universe.Coin.Upbit.Model
{
    [DataContract]
    public class CandleDay : CandleBase//<CandleDay>
    {
        public CandleDay() : base(ApiId.CandleDays, CandleUnit.DAY)
        { 
        }

        /// <summary>
        /// 전일 종가 (UTC 0시 기준)
        /// </summary>
        /// <value>전일 종가 (UTC 0시 기준)</value>
        [DataMember(Name = "prev_closing_price", EmitDefaultValue = false)]
        public decimal PrevClosingPrice { get; set; }

        /// <summary>
        /// 전일 종가 대비 변화 금액
        /// </summary>
        /// <value>전일 종가 대비 변화 금액</value>
        [DataMember(Name = "change_price", EmitDefaultValue = false)]
        public decimal ChangePrice { get; set; }

        /// <summary>
        /// 전일 종가 대비 변화량
        /// </summary>
        /// <value>전일 종가 대비 변화량</value>
        [DataMember(Name = "change_rate", EmitDefaultValue = false)]
        public decimal ChangeRate { get; set; }

        /// <summary>
        /// 종가 환산 화폐 단위로 환산된 가격 (요청에 convertingPriceUnit 파라미터 없을 시 해당 필드 포함되지 않음.) 
        /// </summary>
        /// <value>종가 환산 화폐 단위로 환산된 가격 (요청에 convertingPriceUnit 파라미터 없을 시 해당 필드 포함되지 않음.) </value>
        [DataMember(Name = "converted_trade_price", EmitDefaultValue = false)]
        public decimal ConvertedTradePrice { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.AppendLine($"PrevClosingPrice:    {PrevClosingPrice}");
            sb.AppendLine($"ChangePrice:         {ChangePrice}");
            sb.AppendLine($"ChangeRate:          {ChangeRate}");
            sb.AppendLine($"ConvertedTradePrice: {ConvertedTradePrice}");
            return sb.ToString();
        }

    }//class
}
