using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nachshon.ObjectAccess;
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
