using Newtonsoft.Json;
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
    public class MarketInfo : IApiModel
    {
        public MarketInfo() => Market = KoreanName = EnglishName = MarketWarning = string.Empty;

        /// <summary>
        /// 업비트에서 제공중인 시장 정보
        /// </summary>
        /// <value>업비트에서 제공중인 시장 정보</value>
        [DataMember(Name = "market", EmitDefaultValue = false)]
        public string Market { get; set; }

        /// <summary>
        /// 거래 대상 암호화폐 한글명
        /// </summary>
        /// <value>거래 대상 암호화폐 한글명</value>
        [DataMember(Name = "korean_name", EmitDefaultValue = false)]
        public string KoreanName { get; set; }

        /// <summary>
        /// 거래 대상 암호화폐 영문명
        /// </summary>
        /// <value>거래 대상 암호화폐 영문명</value>
        [DataMember(Name = "english_name", EmitDefaultValue = false)]
        public string EnglishName { get; set; }

        /// <summary>
        /// 유의 종목 여부 - NONE (해당 사항 없음) - CAUTION (투자유의) 
        /// </summary>
        /// <value>유의 종목 여부 - NONE (해당 사항 없음) - CAUTION (투자유의) </value>
        [DataMember(Name = "market_warning", EmitDefaultValue = false)]
        public string MarketWarning { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Market: {Market}");
            sb.AppendLine($"KoreanName: {KoreanName}");
            sb.AppendLine($"EnglishName: {EnglishName}");
            sb.AppendLine($"MarketWarning: {MarketWarning}");
            return sb.ToString();
        }

    }
}
