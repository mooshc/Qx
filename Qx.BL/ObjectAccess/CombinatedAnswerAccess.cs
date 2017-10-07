using Qx.Common;
using NHibernate.Context;

namespace Qx.BL
{
    public class CombinatedAnswerAccess : ObjectAccessNHibernate<CombinatedAnswer>, ICombinatedAnswerAccess
    {
        public CombinatedAnswerAccess(ICurrentSessionContext sessionContext)
            : base(sessionContext)
        {
        }
    }
}
