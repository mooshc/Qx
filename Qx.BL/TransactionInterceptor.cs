using Castle.Core.Interceptor;
using System;
using System.Reflection;
using System.Transactions;

namespace Qx.BL
{
    public class TransactionInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            MethodInfo invocationTarget = invocation.GetConcreteMethodInvocationTarget();
            if (this.IsTransactional(invocationTarget))
            {
                using (TransactionScope transactionScope = this.OpenTrasnaction(invocationTarget))
                {
                    invocation.Proceed();
                    transactionScope.Complete();
                }
            }
            else
                invocation.Proceed();
        }

        private TransactionScope OpenTrasnaction(MethodInfo method)
        {
            object[] customAttributes = method.GetCustomAttributes(typeof(TransactionalAttribute), true);
            if (customAttributes.Length == 0)
                return new TransactionScope();
            TransactionalAttribute transactionalAttribute = customAttributes[0] as TransactionalAttribute;
            return new TransactionScope(transactionalAttribute.Option, new TransactionOptions()
            {
                IsolationLevel = transactionalAttribute.IsolationLevel,
                Timeout = TimeSpan.FromSeconds((double)transactionalAttribute.Timeout)
            });
        }

        private bool IsTransactional(MethodInfo method)
        {
            return method.GetCustomAttributes(typeof(NoTransactionAttribute), false).Length == 0;
        }
    }
}