using Qx.Common;
using NHibernate.Context;

namespace Qx.BL
{
    public class ModuleTypeAccess : ObjectAccessNHibernate<ModuleType> , IModuleTypeAccess
    {
        public ModuleTypeAccess(ICurrentSessionContext sessionContext)
            : base(sessionContext)
        {
        }
    }
}
