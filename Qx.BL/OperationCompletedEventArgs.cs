using System;

namespace Qx.BL
{
    public class OperationCompletedEventArgs : EventArgs
    {
        public bool IsSuccesful { get; set; }

        public OperationCompletedEventArgs(bool isSuccesful)
        {
            this.IsSuccesful = isSuccesful;
        }
    }
}