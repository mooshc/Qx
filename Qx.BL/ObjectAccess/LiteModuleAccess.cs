using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qx.Common;
using Nachshon.ObjectAccess;
using NHibernate.Context;
using NHibernate.Linq;

namespace Qx.BL
{
    public class LiteModuleAccess : ObjectAccessNHibernate<LiteModule>, ILiteModuleAccess
    {
        public LiteModuleAccess(ICurrentSessionContext sessionContext)
            : base(sessionContext)
        {
        }
    }
}
