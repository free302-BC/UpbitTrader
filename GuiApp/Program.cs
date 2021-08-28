using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Universe.AppBase;
using Universe.Coin.App;

namespace Universe.Coin.AppGui
{

    class Program : ProgramBase
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var form = new MainForm();
            AddService<IInputProvider, MainForm>(form);
            AddWorker<TickWorker>("Upbit");
            AddWorkerOption<TickWorker, TickWorkerOptions>("tick.json");
            Task.Run(RunHost);

            form.Size = new System.Drawing.Size(800, 600);
            Application.Run(form);
        }


    }//class
}
