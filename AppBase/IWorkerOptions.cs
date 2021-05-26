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
        public void Reload(IWorkerOptions source);

    }
}
