using Qx.Common;
using NHibernate.Context;

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
