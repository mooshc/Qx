using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nachshon.ObjectAccess;

namespace Qx.Common
{
    public interface IHistoryAccess : IObjectAccess<History>
    {
        void SaveHistory(History h);
        object UsageReport(DateTime startDate, DateTime endDate);
    }
}
