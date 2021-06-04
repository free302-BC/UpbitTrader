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

            //normalize
            var avgUp = ticks.Average(x => x.UnitPrice);
            var sdUp = (decimal)Math.Sqrt((double)ticks.Average(x => (x.UnitPrice - avgUp) * (x.UnitPrice - avgUp)));
            var avgMa = ticks.Average(x => x.MovingAvg);
            var sdMa = (decimal)Math.Sqrt((double)ticks.Average(x => (x.MovingAvg - avgMa) * (x.MovingAvg- avgMa)));
            var avgOsc = ticks.Average(x => x.MacdOsc);
            var sdOsc = (decimal)Math.Sqrt((double)ticks.Average(x => (x.MacdOsc - avgOsc) * (x.MacdOsc - avgOsc)));

            foreach (var t in ticks)
            {
                t.UnitPrice = (t.UnitPrice - avgUp) / sdUp;
                t.MovingAvg = (t.MovingAvg - avgMa) / sdMa;
                t.MacdOsc = (t.MacdOsc - avgOsc) / sdOsc;
            }

            var sb = new StringBuilder();
            for (int i = 0; i < ticks.Length; i++)
            {
                sb.AppendLine($"{ticks[i].UnitPrice}\t{ticks[i].MovingAvg}\t{ticks[i].MacdOsc}");
            }
            File.WriteAllText("ticks.txt", sb.ToString());
            info(IViewModel.Print(ticks));
        }


    }//class
}
