using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nachshon.ObjectAccess;
using Qx.Common;
using NHibernate.Context;
using NHibernate.Linq;

namespace Qx.BL
{
    public class ScenarioAccess : ObjectAccessNHibernate<Scenario>, IScenarioAccess
    {
        public ScenarioAccess(ICurrentSessionContext sessionContext)
            : base(sessionContext)
        {
        }

        public IList<Scenario> GetScenariosByModuleName(string moduleName)
        {
            var s = _sessionContext.CurrentSession();

            return s.Linq<Scenario>().Where(sc => sc.ModuleName == moduleName).ToList();
        }
    }
}
