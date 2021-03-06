﻿using System.Linq;
using Qx.Common;
using NHibernate.Context;
using NHibernate.Linq;

namespace Qx.BL
{
    public class LiteUserAccess : ObjectAccessNHibernate<LiteUser>, ILiteUserAccess
    {
        public LiteUserAccess(ICurrentSessionContext sessionContext)
            : base(sessionContext)
        {
        }

        public LiteUser IsLoginCorrect(string username, string password)
        {
            var s = sessionContext.CurrentSession();
            var lu = s.Linq<LiteUser>().Where(u => u.UserName == username && u.Password == password && !u.IsDeleted && !u.IsLocked).ToList().FirstOrDefault();
            if (lu != null)
            {

                if (ServerStatics.DefaultModules == null)
                    lu.Modules = ServiceLocator.UserAccess.Load(lu.ID).Modules.Select(m => m.Module).ToList();
                else
                    lu.Modules = ServerStatics.DefaultModules;
            }
            return lu;
        }
    }
}
