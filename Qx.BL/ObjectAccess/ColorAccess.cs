
using Qx.Common;
using NHibernate.Context;

namespace Qx.BL
{
    public class ColorAccess : ObjectAccessNHibernate<Color>, IColorAccess
    {
        public ColorAccess(ICurrentSessionContext sessionContext)
            : base(sessionContext)
        {
        }
    }
}
