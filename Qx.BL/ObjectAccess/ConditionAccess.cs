using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qx.Common;
using Nachshon.ObjectAccess;
using NHibernate.Context;

namespace Qx.BL
{
    public class ConditionAccess : ObjectAccessNHibernate<Condition> , IConditionAccess
    {
        public ConditionAccess(ICurrentSessionContext sessionContext)
            : base(sessionContext)
        {
        }
    }
}
