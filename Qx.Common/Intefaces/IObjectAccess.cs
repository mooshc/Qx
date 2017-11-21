using System.Collections.Generic;

namespace Qx.Common
{
    public interface IObjectAccess<T>
    {
        T Load(object id);

        T Save(T obj);

        T SaveOrUpdate(T obj);

        void Update(T obj);

        IList<T> LoadAll();

        IList<T> SaveOrUpdate(IList<T> obj);

        void Delete(object id);
    }
}