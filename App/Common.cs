using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit.App
{
    public class ConfigTuple : Tuple<IConfiguration, IServiceCollection>
    {
        public ConfigTuple(IConfiguration item1, IServiceCollection item2) : base(item1, item2) { }
        public IConfiguration Config => Item1;
        public IServiceCollection Services => Item2;

        public static implicit operator ConfigTuple(ValueTuple<IConfiguration, IServiceCollection> vt) 
            => new ConfigTuple(vt.Item1, vt.Item2);
    }

}
