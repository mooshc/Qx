using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nachshon.Validation;

namespace Qx.Common
{
    [Serializable]
    public class Company : ValidObjectWithIdentity
    {
        public virtual int ID { private set; get; }

        public virtual string Name { set; get; }

        public virtual bool IsDeleted { set; get; }

        protected override object GetObjectId()
        {
            return ID;
        }
    }
}
