using System;
using Xunit;
using Universe.Coin.Upbit.Model;
using Xunit.Abstractions;
using Universe.Coin.TradeLogic.Model;
using System.Linq;
using System.Diagnostics;

namespace UnitTester
{
    public class ViewModelTester
    {
        private readonly Action<object> _out = x => Debug.WriteLine(x);

        public class AM : IApiModel
        {
            public int Id = 123;
            public string Name = "A API Model";
            public override string ToString() => $"Id= {Id}\nName= {Name}";
        }
        public class VM1 : IViewModel<VM1, AM>
        {
            public int Id;
            public string Name;
            public VM1() { }
            public VM1(AM am) => SetApiModel(am);

            public override string ToString() => $"{Id,10} {Name,20}";
            public VM1 SetApiModel(AM apiModel)
            {
                Id = apiModel.Id;
                Name = apiModel.Name;
                return this;
            }

            static VM1() => IViewModel.buildHeader(names);
            static (string, int)[] names = { (nameof(Id), 5), (nameof(Name), 15) };            
        }

        [Fact]
        public void Test1()
        {
            var am = new AM();
            var ams = new[] { am, am };
            _out(IApiModel.Print(ams));

            var ms = ams.Select(x => new VM1(x)).Reverse().ToList();
            //var ms2 = IViewModel<VM1, AM>.ToModels(ams);

            _out(IViewModel.Print((System.Collections.Generic.IEnumerable<IViewModel>)ms));
        }


    }
}
