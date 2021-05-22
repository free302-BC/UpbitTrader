using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit.Model
{
    /// <summary>
    /// 계산용 모델(xxxModel)의 베이스
    /// </summary>
    public abstract class ViewModelBase<VM, AM> : IViewModel
        where VM : ViewModelBase<VM, AM>, new()
        where AM : IApiModel
    {
        public static List<VM> ToModels(IEnumerable<AM> models)
            => models.Select(x => ToModel(x)).Reverse().ToList();

        public static VM ToModel(AM apiModel) => new VM().setApiModel(apiModel);

        protected abstract VM setApiModel(AM apiModel);

    }//class
}
