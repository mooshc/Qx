using Qx.Common;
using NHibernate.Context;

namespace Qx.BL
{
    public class CompanyAccess : ObjectAccessNHibernate<Company> , ICompanyAccess
    {
        public CompanyAccess(ICurrentSessionContext sessionContext)
            : base(sessionContext)
        {
        }
    }
}
