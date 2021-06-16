﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Universe.AppBase;
using Universe.Coin.TradeLogic;

namespace Universe.Coin.App
{
    public class WorkerOptions : ITradeOptions, IWorkerOptions
    {
        public WorkerOptions()
        {
            AssemblyFile = "";
            ClientFullName = "";
            AccessKey = "";
            SecretKey = "";
        }
        public string AssemblyFile { get; set; }
        public string ClientFullName { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }

        public void Reload(IWorkerOptions source)
        {
            IWorkerOptions.checkType(this, source);
            var src = (WorkerOptions)source;
            AssemblyFile = src.AssemblyFile;
            AccessKey = src.AccessKey;
            SecretKey = src.SecretKey;
        }

        public IWorkerOptions Clone()
        {
            var dest = new WorkerOptions();
            dest.AssemblyFile = AssemblyFile;
            dest.AccessKey = AccessKey;
            dest.SecretKey = SecretKey;
            return dest;
        }

    }//class    
}
