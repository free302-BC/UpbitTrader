using System;
using Xunit;
using Universe.Coin.Upbit.Model;
using Xunit.Abstractions;

namespace UnitTester
{
    public class ModelTester
    {
        private readonly ITestOutputHelper _out;
        public ModelTester(ITestOutputHelper output) => _out = output;

        public class AM : IApiModel 
        {
            public int Id = 123;
            public string Name = "A API Model";
            public override string ToString() => $"Id= {Id}\nName= {Name}";
        }
        public class M : ViewModelBase<M, AM>
        {
            protected override M setApiModel(AM apiModel)
            {
                Id = apiModel.Id;
                Name = apiModel.Name;
                return this;
            }

            static M() => IViewModel.buildHeader(names);
            static (string, int)[] names = { (nameof(Id), 10), (nameof(Name), 20) };

            int Id;
            string Name;
            public override string ToString() => $"CmId= {Id}, CmName= {Name}";
        }

        [Fact]
        public void Test1()
        {
            var am = new AM();
            var ams = new[] { am, am };
            _out.WriteLine(IApiModel.Print(ams));
            var ms = M.ToModels(ams);
            _out.WriteLine(IViewModel.Print(ms));
        }


    }
}
