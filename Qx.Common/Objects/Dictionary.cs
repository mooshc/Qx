using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nachshon.Validation;

namespace Qx.Common
{
    [Serializable]
    public class Dictionary : ValidObjectWithIdentity
    {
        public virtual int ID { private set; get; }

        public virtual string ObjectName { set; get; }

        public virtual string Text { set; get; }

        public virtual Language Language { set; get; }

        protected override object GetObjectId()
        {
            return ID;
        }
    }
}
