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
            foreach (var h in names) sb.AppendFormat($"{{0,{h.wdith}}} ", h.name);
            _header = sb.ToString();
        }        

        public static string Print(IEnumerable<IViewModel> models, bool printHeader = true)
        {
            var sb = new StringBuilder();
            if(printHeader) sb.AppendLine(_header);
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

}
