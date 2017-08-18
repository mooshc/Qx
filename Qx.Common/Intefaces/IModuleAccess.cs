using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nachshon.ObjectAccess;

namespace Qx.Common
{
    public interface IModuleAccess : IObjectAccess<Module>
    {
        List<Module> GetModulesByIDs(List<int> IDs);
        List<Module> GetPhysicalExModules();
        List<string> GetModulesNames();
        Module LoadModuleByName(string name);
        List<string> GetAnamnesisModules();
        List<Module> GetAnamnesisRealModules();
        bool LoadToServerByName(string name);
    }
}
