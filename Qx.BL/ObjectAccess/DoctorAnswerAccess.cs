using Qx.Common;
using NHibernate.Context;

namespace Qx.BL
{
    public class DoctorAnswerAccess : ObjectAccessNHibernate<DoctorAnswer> , IDoctorAnswerAccess
    {
        public DoctorAnswerAccess(ICurrentSessionContext sessionContext)
            : base(sessionContext)
        {
        }
    }
}
