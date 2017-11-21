using System;
using System.Security.Permissions;

namespace Qx.BL
{
    public abstract class MarshalByRefSingleton : MarshalByRefObject
    {
        [SecurityPermission(SecurityAction.LinkDemand, Infrastructure = true)]
        public override object InitializeLifetimeService()
        {
            return (object)null;
        }
    }
}