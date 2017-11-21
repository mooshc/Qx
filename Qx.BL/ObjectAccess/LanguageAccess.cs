using Qx.Common;
using NHibernate.Context;

namespace Qx.BL
{
    public class LanguageAccess : ObjectAccessNHibernate<Language> , ILanguageAccess
    {
        public LanguageAccess(ICurrentSessionContext sessionContext)
            : base(sessionContext)
        {
        }
    }
}
