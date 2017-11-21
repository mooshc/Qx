using NHibernate;
using NHibernate.Context;
using Qx.Common;
using System.Collections.Generic;

namespace Qx.BL
{
    public class ObjectAccessNHibernate<T> : IObjectAccess<T>
    {
        protected ICurrentSessionContext sessionContext;

        protected ICriteria Criteria
        {
            get
            {
                return this.sessionContext.CurrentSession().CreateCriteria(typeof(T));
            }
        }

        public ObjectAccessNHibernate(ICurrentSessionContext session)
        {
            sessionContext = session;
            //sessionContext.CurrentSession().FlushMode = FlushMode.Always;
        }

        public T Load(object id)
        {
            var obj =  sessionContext.CurrentSession().Load<T>(id);
            NHibernateUtil.Initialize(obj);
            return obj;
        }

        public T Save(T obj)
        {
            int index = (int)sessionContext.CurrentSession().Save(obj);
            return Load(index);
        }

        public T SaveOrUpdate(T obj)
        {
            var currentSession = sessionContext.CurrentSession();
            try
            {
                currentSession.SaveOrUpdate(obj);
            }
            catch
            {
                object identifier = currentSession.GetIdentifier(obj);
                if (identifier != null && Load(identifier) != null)
                    currentSession.Update(obj);
                else
                    currentSession.Save(obj);
            }
            return obj;
        }

        public void Update(T obj)
        {
            sessionContext.CurrentSession().Update(obj);
        }

        public virtual IList<T> LoadAll()
        {
            return this.Criteria.List<T>();
        }

        public IList<T> SaveOrUpdate(IList<T> obj)
        {
            var results = new List<T>();
            foreach (T item in obj)
            {
                results.Add(SaveOrUpdate(item));
            }

            return results;
        }

        public void Delete(object id)
        {
            sessionContext.CurrentSession().Delete(id);
        }
    }
}