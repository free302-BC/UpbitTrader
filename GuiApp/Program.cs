using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Universe.AppBase;

namespace Universe.Coin.App
{
    using EventDic = Dictionary<ConsoleKey, CommandListener>;

    class Program : ProgramBase, ICommandProvider
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

            AddService<ICommandProvider, Program>(false);
            AddWorker<TickWorker, TickWorkerOptions>("tick.json", "Upbit");

            Task.Run(RunHost);

            var form = new Form();
            form.Size = new System.Drawing.Size(800, 600);
            Application.Run(form);
        }

        
        readonly EventDic _listeners;
        readonly object _lock;

        public Program()
        {
            _listeners = new();
            _lock = new();
        }

        public void AddCmd(ConsoleKey key, CommandListener cmd)
        {
            lock (_lock)
            {
                if (_listeners.ContainsKey(key)) _listeners[key] += cmd;
                else _listeners[key] = cmd;
            }
        }
        public void RemoveCmd(ConsoleKey key, CommandListener cmd)
        {
            lock (_lock)
            {
                if (_listeners.ContainsKey(key))
                {
                    var result = _listeners[key] - cmd;
                    if (result is null) _listeners.Remove(key);
                    else _listeners[key] = result;
                }
            }
        }



    }//class
}
