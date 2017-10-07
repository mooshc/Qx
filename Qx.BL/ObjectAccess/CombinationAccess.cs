using Qx.Common;
using NHibernate.Context;

namespace Qx.BL
{
    public class CombinationAccess : ObjectAccessNHibernate<Combination> , ICombinationAccess
    {
        public CombinationAccess(ICurrentSessionContext sessionContext)
            : base(sessionContext)
        {
        }
    }
}
