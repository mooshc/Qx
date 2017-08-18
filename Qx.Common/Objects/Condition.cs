using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nachshon.Validation;

namespace Qx.Common
{
    [Serializable]
    public class Condition : ValidObjectWithIdentity
    {
        public virtual int ID { private set; get; }

        public virtual string Name { set; get; }

        public virtual char ConditionType { set; get; }

        public virtual int Value { set; get; }

        public virtual int SecondValue { set; get; }

        public virtual Color Color { set; get; }

        public virtual bool IsDeleted { set; get; }

        public virtual string ConditionHebText
        {
            set
            {
                var lang = CommonFunctions.HebLang;
                RemoteObjectProvider.GetDictionaryAccess().SaveOrUpdateByName(Name, value, lang);
                ContentDictionary.SaveOrUpdateValue(Name, value, lang);
            }
            get
            {
                return ContentDictionary.GetContent(Name, CommonFunctions.HebLang);
            }
        }

        protected override object GetObjectId()
        {
            return ID;
        }
    }
}
