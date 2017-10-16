using System;

namespace Qx.Common
{
    [Serializable]
    public class Dictionary
    {
        public virtual int ID { set; get; }

        public virtual string ObjectName { set; get; }

        public virtual string Text { set; get; }

        public virtual Language Language { set; get; }
    }
}
