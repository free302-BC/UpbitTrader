using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit.Model
{
    /// <summary>
    /// 계산용 모델의 베이스
    /// </summary>
    public abstract class ModelBase : IModel, IPrint
    {
        static ModelBase()
        {
            
        }

        public static string Print(IEnumerable<TickerModel> models)
        {
            var sb = new StringBuilder();
            //sb.AppendLine(_header);
            foreach (var m in models) sb.AppendLine(m.ToString());
            return sb.ToString();
        }

    }//class
}
