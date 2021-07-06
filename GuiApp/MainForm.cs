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
using Universe.Utility;

namespace Universe.Coin.AppGui
{
    using SignalDic = SortedMultiDictionary<ConsoleKey, ConsoleModifiers, List<EventWaitHandle>>;

    public partial class MainForm : Form, IInputProvider
    {
        public MainForm()
        {
            InitializeComponent();
            KeyPreview = true;
            _signals = new();
            _lock = new();            
        }

        readonly SignalDic _signals;
        readonly object _lock;
        SignalDic IInputProvider._signals => _signals;
        object IInputProvider._lock => _lock;

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            var key = (ConsoleKey)e.KeyCode;
            var mod = (ConsoleModifiers)e.Modifiers;
            //CommandAction? cmd = null;
            List<EventWaitHandle>? list = null;
            lock (_lock)
            {
                if (_signals.ContainsKey(key, mod)) list = this._signals[key, mod];
            }

            try
            {
                //cmd?.Invoke(ki.Modifiers);
                if (list != null) foreach (var h in list) h.Set();
            }
            catch (Exception ex)
            {
                logEx(e, ex);
            }
            void logEx(KeyEventArgs ki, Exception? ex)
            {
                var key = ki.Modifiers == 0 ? ki.KeyCode.ToString() : $"{ki.Modifiers}, {ki.KeyCode}";
                log($"Error executing <{key}>", ex);
            }

        }

        void log(object? msg, Exception? ex = null)
        {
            MessageBox.Show($"{msg}\r\n{ex?.Message}");
        }

    }//class
}
