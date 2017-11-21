using System.Collections.Generic;

namespace Qx.Common
{
    public interface IScenarioAccess : IObjectAccess<Scenario>
    {
        IList<Scenario> GetScenariosByModuleName(string moduleName);
    }
}
