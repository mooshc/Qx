using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qx.Common;
using Nachshon.ObjectAccess;
using NHibernate.Context;
using NHibernate.Linq;
using System.Data;
using System.Data.SqlClient;

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
