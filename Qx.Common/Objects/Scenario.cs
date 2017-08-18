﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nachshon.Validation;

namespace Qx.Common
{
    [Serializable]
    public class Scenario : ValidObjectWithIdentity
    {
        public virtual int ID { private set; get; }

        public virtual string Name { set; get; }

        public virtual string FileName { set; get; }

        public virtual string ModuleName { set; get; }

        public virtual bool IsTest { set; get; }

        public virtual bool IsDeleted { set; get; }

        public virtual IList<DoctorAnswer> AnamnesisAnswers { set; get; }

        public virtual IList<DoctorAnswer> PhysicalExAnswers { set; get; }

        public virtual string ModuleHebName { get { return ContentDictionary.GetContent(ModuleName, null); } }

        protected override object GetObjectId()
        {
            return ID;
        }
    }
}
