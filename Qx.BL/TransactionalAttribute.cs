using System;
using System.Transactions;

namespace Qx.BL
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TransactionalAttribute : Attribute
    {
        public int Timeout { get; set; }

        public TransactionScopeOption Option { get; set; }

        public IsolationLevel IsolationLevel { get; set; }

        public TransactionalAttribute()
        {
            this.Timeout = 90;
            this.Option = TransactionScopeOption.Required;
            this.IsolationLevel = IsolationLevel.Serializable;
        }
    }
}