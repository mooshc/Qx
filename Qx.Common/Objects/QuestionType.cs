using System;

namespace Qx.Common
{
    [Serializable]
    public class QuestionType 
    {
        public virtual int ID { private set; get; }

        public virtual string Name { set; get; }

        public virtual bool IsDeleted { set; get; }
    }
}
