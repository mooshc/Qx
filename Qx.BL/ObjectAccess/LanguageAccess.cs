using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qx.Common;
using Nachshon.ObjectAccess;
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
