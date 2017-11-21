using System.Collections.Generic;
using System.Linq;
using Qx.Common;
using NHibernate.Context;
using NHibernate.Linq;

namespace Qx.BL
{
    public class ModuleAccess : ObjectAccessNHibernate<Module> , IModuleAccess
    {
        public ModuleAccess(ICurrentSessionContext sessionContext)
            : base(sessionContext)
        {
        }

        public List<Module> GetModulesByIDs(List<int> IDs)
        {
            var s = sessionContext.CurrentSession();

            return s.Linq<Module>().Where(m => IDs.Contains(m.ID)).ToList();
        }

        public List<Module> GetPhysicalExModules()
        {
            var s = sessionContext.CurrentSession();

            return s.Linq<Module>().Where(m => m.ModuleType.ID == 2 && !m.IsDeleted).ToList();
        }
        
        public List<Module> GetAnamnesisRealModules()
        {
            var s = sessionContext.CurrentSession();

            return s.Linq<Module>().Where(m => m.ModuleType.ID == 1 && !m.IsDeleted).ToList();
        }

        public List<string> GetModulesNames()
        {
            var s = sessionContext.CurrentSession();

            return s.Linq<Module>().Where(mo => !mo.IsDeleted).Select(m => m.Name).ToList();
        }

        public List<string> GetAnamnesisModules()
        {
            var s = sessionContext.CurrentSession();

            return s.Linq<Module>().Where(m => m.ModuleType.ID == 1 && !m.IsDeleted).Select(m => m.Name).ToList();
        }

        public Module LoadModuleByName(string name)
        {
            var s = sessionContext.CurrentSession();

            return s.Linq<Module>().Where(m => m.Name == name).ToList().FirstOrDefault();
        }

        public bool LoadToServerByName(string name)
        {
            try
            {
                if (ServerStatics.DefaultModules == null)
                    ServerStatics.DefaultModules = new List<Module>();
                var s = sessionContext.CurrentSession();

                var mod = s.Linq<Module>().Where(m => m.Name == name).ToList().FirstOrDefault();
                ServerStatics.DefaultModules.Add(mod);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
