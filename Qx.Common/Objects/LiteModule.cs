using Qx.Common.Objects;
using System;

namespace Qx.Common
{
    [Serializable]
    public class LiteModule : TranslatedObject
    {
        public virtual int ID { set; get; }

        public virtual string Name { set; get; }

        public virtual ModuleType ModuleType { set; get; }

        public virtual string Tags { set; get; }

        public virtual bool? IsMale { set; get; }

        public virtual bool IsDeleted { set; get; }

        public virtual string ModuleHebText
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

        public LiteModule()
        {
        }
    }
}
