using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit.Model
{
    /// <summary>
    /// API 리턴 모델 + Calc model
    /// </summary>
    public interface IModel
    {
    }

    public interface IViewModel : IModel
    {
        static string? _header;
        protected static void buildHeader((string name, int wdith)[] names)
        {
            StringBuilder sb = new();
            foreach (var h in names) sb.Append($"{{{h.name},{h.wdith}}} ");
            _header = sb.ToString();
        }
        protected static void buildHeader((string name, int wdith, string fmt)[] names)
        {
            StringBuilder sb = new();
            foreach (var h in names) sb.Append($"{{{h.name},{h.wdith}:{h.fmt}}} ");
            _header = sb.ToString();
        }

        public static string Print(IEnumerable<IViewModel> models)
        {
            var sb = new StringBuilder();
            sb.AppendLine(_header);
            foreach (var m in models) sb.AppendLine(m.ToString());
            return sb.ToString();
        }
    }

    /// <summary>
    /// API 리턴 모델
    /// </summary>
    public interface IApiModel : IModel
    {
        public static string Print(IEnumerable<IApiModel> models)
        {
            var sb = new StringBuilder();
            foreach (var m in models) sb.AppendLine(m.ToString());
            return sb.ToString();
        }
    }

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
