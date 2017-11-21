using NHibernate;
using NHibernate.Context;
using NHibernate.Engine;
using System;

namespace Qx.BL
{
    public class OperationSessionProvider : ICurrentSessionContext
    {
        private ISessionFactoryImplementor _isf;

        public Func<IInterceptor> InterceptorFactory { get; set; }

        public OperationSessionProvider(ISessionFactoryImplementor isf)
        {
            this._isf = isf;
        }

        public ISession CurrentSession()
        {
            if (!Operation.Current.OperationContext.Contains((object)this._isf))
            {
                IInterceptor sessionLocalInterceptor = (IInterceptor)null;
                if (this.InterceptorFactory != null)
                    sessionLocalInterceptor = this.InterceptorFactory();
                ISession session = sessionLocalInterceptor == null ? this._isf.OpenSession() : this._isf.OpenSession(sessionLocalInterceptor);
                Operation.Current.Completed += (EventHandler<OperationCompletedEventArgs>)((s, e) =>
                {
                    if (e.IsSuccesful)
                        session.Flush();
                    session.Dispose();
                });
                Operation.Current.OperationContext.Add((object)this._isf, (object)session);
            }
            return (ISession)Operation.Current.OperationContext[(object)this._isf];
        }
    }
}