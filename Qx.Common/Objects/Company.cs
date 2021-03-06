﻿using System;

namespace Qx.Common
{
    [Serializable]
    public class Company
    {
        public virtual int ID { set; get; }

        public virtual string Name { set; get; }

        public virtual bool IsDeleted { set; get; }
    }
}
