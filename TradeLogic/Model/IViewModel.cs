using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic.Model
{
    /// <summary>
    /// 모든 ViewModel 인터페이스, Print 기본 구현
    /// </summary>
    public interface IViewModel : IModel
    {
        #region ---- Print ----

        static string? _header;
        protected static void buildHeader((string name, int wdith)[] names)
        {
            StringBuilder sb = new();
            foreach (var h in names) sb.AppendFormat($"{{0,{h.wdith}}} ", h.name);
            _header = sb.ToString();
        }

        public static string Print(IEnumerable<IViewModel> models, bool printHeader = true)
        {
            return Print(models, 0, models.Count(), printHeader);
        }
        public static string Print(IEnumerable<IViewModel> models, int offset, int count, bool printHeader = true)
        {
            var sb = new StringBuilder();
            if (printHeader) sb.AppendLine(_header);
            foreach (var m in models.Skip(offset).Take(count)) sb.AppendLine(m.ToString());
            return sb.ToString();
        }
        #endregion

    }//ViewModel
    
}
