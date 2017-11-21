using Qx.Common;
using NHibernate.Context;

namespace Qx.BL
{
    public class LiteQuestionAccess : ObjectAccessNHibernate<LiteQuestion>, ILiteQuestionAccess
    {
        public LiteQuestionAccess(ICurrentSessionContext sessionContext)
            : base(sessionContext)
        {
        }
    }
}
