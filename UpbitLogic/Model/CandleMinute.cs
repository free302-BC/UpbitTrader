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
    public class CandleMinute : CandleBase<CandleMinute>
    {
        public CandleMinute() { }
        public CandleMinute(CandleUnit unit) : base(unit) { }
        public CandleMinute(int unit) : base((CandleUnit)unit) { }
        public override CandleUnit CandleUnit
        {
            get => (CandleUnit)Unit;
            set
            {
                ICandle.CheckParam<CandleMinute>(value);
                Unit = (decimal)value;
            }
        }

        /// <summary>
        /// 분 단위(유닛)
        /// </summary>
        /// <value>분 단위(유닛)</value>
        [DataMember(Name = "unit", EmitDefaultValue = false)]
        public decimal Unit { get; set; }        

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.AppendLine($"Unit:    {Unit}");
            return sb.ToString();
        }

    }//class
}
