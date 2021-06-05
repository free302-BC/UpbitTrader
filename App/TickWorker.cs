using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.AppBase;
using Universe.Coin.TradeLogic.Calc;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.Upbit.App
{
    public class TickWorker : WorkerBase<TickWorker, BackTestOptions>
    {
        public TickWorker(
            ILogger<TickWorker> logger, 
            IServiceProvider sp, 
            IOptionsMonitor<BackTestOptions> set)
            : base(logger, sp, set)
        {
        }

        protected override void work()
        {
            var logger = _sp.GetRequiredService<ILogger<Client>>();
            var uc = new Client(_set.AccessKey, _set.SecretKey, logger);
            while (true)
            {
                run_Tick_K(uc);
                break;
            }
        }
        void run_Tick_K(Client uc)
        {
            var param = _set.CalcParam;
            var ticks = uc.ApiTicks(count: 50).ToModels();
            ICalcTradeTick.CalcMovingAvg(ticks, param);
            ICalcTradeTick.CalcMacdOsc(ticks, param);
            //ICalcTradeTick.CalcProfitRate(ticks, param);
            //ICalcTradeTick.CalcCumRate(ticks);
            //ICalcTradeTick.CalcDrawDown(ticks);
            
            info(IViewModel.Print(ticks));
        }


    }//class
}
