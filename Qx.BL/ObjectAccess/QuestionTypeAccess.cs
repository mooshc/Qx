using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qx.Common;
using Nachshon.ObjectAccess;
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
