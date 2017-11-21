using Qx.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qx.Client
{
    class HistoryLogger : IDisposable
    {
        private readonly Mutex mutex;
        private readonly string filePath;
        private readonly string qxdbVersion;
        private readonly string qxdbType;
        private readonly ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
    
        public HistoryLogger(string filePath, string qxdbVersion, string qxdbType)
        {
            mutex = new Mutex(false, "Qx.HistoryLogger");
            this.filePath = filePath;
            this.qxdbVersion = qxdbVersion;
            this.qxdbType = qxdbType;
        }

        public void Log(History history)
        {
            
        }

        public void Dispose()
        {
            mutex.Dispose();
        }
    }
}
