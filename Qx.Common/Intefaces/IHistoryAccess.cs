using System;

namespace Qx.Common
{
    public interface IHistoryAccess : IObjectAccess<History>
    {
        void SaveHistory(History h);
        object UsageReport(DateTime startDate, DateTime endDate);
    }
}
