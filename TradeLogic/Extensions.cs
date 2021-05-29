using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic
{
    public static class Extensions
    {
        public static string Print(this IEnumerable<IViewModel> models) => IViewModel.Print(models);
        public static string Print(this IEnumerable<IViewModel> models, int offset, int count) 
            => IViewModel.Print(models, offset, count);

    }
}
