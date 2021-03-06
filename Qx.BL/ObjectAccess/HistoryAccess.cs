﻿using System;
using Qx.Common;
using NHibernate.Context;
using System.Collections;

namespace Qx.BL
{
    public class HistoryAccess : ObjectAccessNHibernate<History> , IHistoryAccess
    {
        public HistoryAccess(ICurrentSessionContext sessionContext)
            : base(sessionContext)
        {
        }

        public void SaveHistory(History h)
        {
            Save(h);
        }

        public object UsageReport(DateTime startDate, DateTime endDate)
        {
            string dateFormat = "yyyy-MM-dd";
            var s = sessionContext.CurrentSession();
            IList result = s.CreateSQLQuery(string.Format("call `AllUsageByTimespan`('{0}', '{1}');", startDate.ToString(dateFormat), endDate.ToString(dateFormat))).List();

            return result;
        }
    }
}
