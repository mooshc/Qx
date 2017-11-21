using System;
using System.Collections.Generic;

namespace Qx.Common
{
    [Serializable]
    public class LiteUser
    {
        public virtual int ID { set; get; }

        public virtual Guid Guid { set; get; }

        public virtual string UserName { set; get; }

        public virtual string FirstName { set; get; }

        public virtual string LastName { set; get; }

        public virtual string Password { set; get; }

        public virtual Company Company { set; get; }

        public virtual Language Language { set; get; }

        public virtual bool IsLocked { set; get; }

        public virtual bool IsAdmin { set; get; }

        public virtual bool IsDeleted { set; get; }

        public virtual string License { set; get; }
        public virtual string PID { set; get; }
        public virtual string Field1 { set; get; }
        public virtual string Field2 { set; get; }
        public virtual string Field3 { set; get; }
        public virtual string Field4 { set; get; }

        public virtual IList<Module> Modules { set; get; }

        public LiteUser()
        {
            Guid = Guid.NewGuid();
            Modules = new List<Module>();
        }

        public override bool Equals(object obj)
        {
            return obj is User && (obj as User).ID == ID;
        }
    }
}
