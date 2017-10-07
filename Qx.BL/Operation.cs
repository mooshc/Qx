using System;
using System.Collections;

namespace Qx.BL
{
    public class Operation : IDisposable
    {
        [ThreadStatic]
        private static Operation _currentOperation;

        public static Operation Current
        {
            get
            {
                return Operation._currentOperation;
            }
        }

        public bool IsCompleted { get; private set; }

        public Operation()
        {
            if (Operation._currentOperation != null)
                throw new InvalidOperationException("Only one operation per thread is allowed");
            Operation._currentOperation = this;
            this.OperationContext = (IDictionary)new Hashtable();
        }

        public IDictionary OperationContext { get; private set; }

        public event EventHandler<OperationCompletedEventArgs> Completed;

        public void Complete()
        {
            this.Finish(true);
        }

        public void Dispose()
        {
            if (this.IsCompleted)
                return;
            this.Finish(false);
        }

        private void Finish(bool isSuccesful)
        {
            if (this.IsCompleted)
                throw new InvalidOperationException("Object is disposed or finished");
            this.IsCompleted = true;
            Operation._currentOperation = (Operation)null;
            if (this.Completed == null)
                return;
            this.Completed((object)this, new OperationCompletedEventArgs(isSuccesful));
        }
    }
}