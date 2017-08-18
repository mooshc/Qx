﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qx.Common;
using Nachshon.ObjectAccess;
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
