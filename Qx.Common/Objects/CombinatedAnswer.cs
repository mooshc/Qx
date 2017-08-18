using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nachshon.Validation;

namespace Qx.Common
{
    [Serializable]
    public class CombinatedAnswer : ValidObjectWithIdentity
    {
        public virtual int ID { private set; get; }

        public virtual Combination Combination { set; get; }

        public virtual Answer Answer { set; get; }

        public virtual bool IsNot { set; get; }

        public virtual bool IsDeleted { set; get; }

        public CombinatedAnswer()
        {
        }

        protected override object GetObjectId()
        {
            return ID;
        }

        public override bool Equals(object obj)
        {
            return obj is CombinatedAnswer && (obj as CombinatedAnswer).Answer == Answer;
        }
    }
}
