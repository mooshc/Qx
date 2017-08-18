using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qx.Common;
using Nachshon.ObjectAccess;
using NHibernate.Context;
using NHibernate.Linq;
using Nachshon.Proxy;

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
            var s = _sessionContext.CurrentSession();

            return s.Linq<Module>().Where(m => IDs.Contains(m.ID)).ToList();
        }

        public List<Module> GetPhysicalExModules()
        {
            var s = _sessionContext.CurrentSession();

            return s.Linq<Module>().Where(m => m.ModuleType.ID == 2 && !m.IsDeleted).ToList();
        }
        [NoTransaction]
        public List<Module> GetAnamnesisRealModules()
        {
            var s = _sessionContext.CurrentSession();

            return s.Linq<Module>().Where(m => m.ModuleType.ID == 1 && !m.IsDeleted).ToList();
        }

        public List<string> GetModulesNames()
        {
            var s = _sessionContext.CurrentSession();

            return s.Linq<Module>().Where(mo => !mo.IsDeleted).Select(m => m.Name).ToList();
        }

        public List<string> GetAnamnesisModules()
        {
            var s = _sessionContext.CurrentSession();

            return s.Linq<Module>().Where(m => m.ModuleType.ID == 1 && !m.IsDeleted).Select(m => m.Name).ToList();
        }

        public Module LoadModuleByName(string name)
        {
            var s = _sessionContext.CurrentSession();

            return s.Linq<Module>().Where(m => m.Name == name).ToList().FirstOrDefault();
        }

        public bool LoadToServerByName(string name)
        {
            try
            {
                if (ServerStatics.DefaultModules == null)
                    ServerStatics.DefaultModules = new List<Module>();
                var s = _sessionContext.CurrentSession();

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
