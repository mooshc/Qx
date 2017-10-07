using System;
using System.Linq;
using Qx.Common;
using NHibernate.Context;
using NHibernate.Linq;

namespace Qx.BL
{
    public class UserAccess : ObjectAccessNHibernate<User> , IUserAccess
    {
        public UserAccess(ICurrentSessionContext sessionContext)
            : base(sessionContext)
        {
        }

        public User IsLoginCorrect(string username, string password)
        {
            var s = sessionContext.CurrentSession();

            return s.Linq<User>().Where(u => u.UserName == username && u.Password == password && !u.IsDeleted && !u.IsLocked).ToList().FirstOrDefault();
        }

        public bool IsGuidCorrect(Guid guid)
        {
            var s = sessionContext.CurrentSession();
            var result = s.CreateSQLQuery("call `IsGuidCorrect`('" + guid.ToString() + "');").List<long>();
            return result.FirstOrDefault() > 0;
        }

        public bool IsAdminUser(Guid guid)
        {
            var s = sessionContext.CurrentSession();
            var result = s.CreateSQLQuery("call `IsAdminUser`('" + guid.ToString() + "');");
            return result.List<long>().FirstOrDefault() > 0;
        }

        public void PermitUserToAnamnesis(User user, Module anamnesis)
        {
            foreach (var mod in user.Modules)
            {
                if (mod.Module == anamnesis)
                    mod.IsAuthorized = true;
            }
            SaveOrUpdate(user);
        }
    }
}
