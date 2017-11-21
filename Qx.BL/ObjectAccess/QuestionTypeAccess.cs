using Qx.Common;
using NHibernate.Context;

namespace Qx.BL
{
    public class QuestionTypeAccess : ObjectAccessNHibernate<QuestionType> , IQuestionTypeAccess
    {
        public QuestionTypeAccess(ICurrentSessionContext sessionContext)
            : base(sessionContext)
        {
        }
    }
}
