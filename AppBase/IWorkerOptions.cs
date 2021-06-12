using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.AppBase
{
    public interface IWorkerOptions
    {
        /// <summary>
        /// source의 내용을 반영
        /// </summary>
        /// <param name="source"></param>
        void Reload(IWorkerOptions source);

        /// <summary>
        /// source의 내용을 복제 - 구현클래스에서 딥카피
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        IWorkerOptions Clone();

        protected static void checkType(IWorkerOptions dest, IWorkerOptions source)
        {
            var srcT = source.GetType();
            var destT = dest.GetType();
            if (srcT != destT)
                throw new ArgumentException(
                    $"{destT.Name}.{nameof(Reload)}(): can't reload from type <{srcT.Name}>");
        }
    }
}
