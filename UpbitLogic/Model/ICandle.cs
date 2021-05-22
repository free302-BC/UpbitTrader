using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit.Model
{
    /// <summary>
    /// Candle API의 리턴 모델 - CandleModel로 변환시
    /// </summary>
    public interface ICandle : IApiModel
    {
        public string CandleDateTimeKst { get; set; }
        public decimal OpeningPrice { get; set; }
        public decimal HighPrice { get; set; }
        public decimal LowPrice { get; set; }
        public decimal TradePrice { get; set; }

    }//class
}
