using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic.Model
{
    /// <summary>
    /// API 리턴 모델 + Calc model
    /// </summary>
    public interface IModel
    {
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
