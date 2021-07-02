using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Universe.Coin.App;

namespace Universe.Coin.AppGui
{
    using EventDic = Dictionary<ConsoleKey, CommandListener>;

    public partial class MainForm : Form, ICommandProvider
    {
        public MainForm()
        {
            InitializeComponent();

            _listeners = new();
            _lock = new();
        }

        readonly EventDic _listeners;
        readonly object _lock;

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
    }
}
