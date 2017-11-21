using Qx.Common.Objects;
using System;

namespace Qx.Common
{
    [Serializable]
    public class Condition : TranslatedObject
    {
        public virtual int ID { set; get; }

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
                SaveOrUpdateByName(Name, value, lang);
            }
            get
            {
                return ContentDictionary.GetContent(Name, CommonFunctions.HebLang);
            }
        }
    }
}
