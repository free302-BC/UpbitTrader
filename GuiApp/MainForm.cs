using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Universe.Coin.App;

namespace Universe.Coin.AppGui
{
    using EventDic = Dictionary<ConsoleKey, CommandAction>;

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

        public void AddSignal(ConsoleKey key, EventWaitHandle signal)
        {
            throw new NotImplementedException();
        }

        public void RemoveSignal(ConsoleKey key, EventWaitHandle signal)
        {
            throw new NotImplementedException();
        }
    }
}
