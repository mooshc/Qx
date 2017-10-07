using Qx.Common;
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
