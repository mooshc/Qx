using System;

namespace Qx.Common
{
    [Serializable]
    public class CombinatedAnswer
    {
        public virtual int ID { set; get; }

        public virtual Combination Combination { set; get; }

        public virtual Answer Answer { set; get; }

        public virtual bool IsNot { set; get; }

        public virtual bool IsDeleted { set; get; }

        public CombinatedAnswer()
        {
        }

        public override bool Equals(object obj)
        {
            return obj is CombinatedAnswer && (obj as CombinatedAnswer).Answer == Answer;
        }
    }
}
