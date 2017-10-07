using System;

namespace Qx.Common
{
    [Serializable]
    public class ModuleInUser
    {
        public virtual Module Module { set; get; }
        public virtual bool IsAuthorized { set; get; }

        public ModuleInUser()
        {

        }

        public ModuleInUser(Module module, bool isAuthorized)
        {
            Module = module;
            IsAuthorized = isAuthorized;
        }

        public override bool Equals(object obj)
        {
            try
            {
                return (obj as ModuleInUser).Module == Module;
            }
            catch
            {
                return false;
            }
        }
    }
}
