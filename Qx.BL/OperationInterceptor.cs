using Castle.Core.Interceptor;
using log4net;
using System;
using System.Reflection;

namespace Qx.BL
{
    public class OperationInterceptor : IInterceptor
    {
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Intercept(IInvocation invocation)
        {
            LogicalThreadContext.Properties["SessionId"] = (object)Guid.NewGuid();
            if (LogicalThreadContext.Properties["CallId"] == null)
            {
                LogicalThreadContext.Properties["CallId"] = (object)Guid.NewGuid();
                LogicalThreadContext.Properties["TopLevelType"] = (object)invocation.Method.ReflectedType.ToString();
                LogicalThreadContext.Properties["TopLevelMethod"] = (object)invocation.Method.ToString();
            }
            log.Info((object)"Start call");
            try
            {
                using (Operation operation = new Operation())
                {
                    invocation.Proceed();
                    if (operation.IsCompleted)
                        return;
                    operation.Complete();
                }
            }
            catch (Exception ex)
            {
                log.Warn((object)"exception during call invocation", ex);
                throw;
            }
            finally
            {
                log.Info((object)"End call");
                LogicalThreadContext.Properties.Clear();
            }
        }
    }
}