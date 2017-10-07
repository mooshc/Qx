using System;

namespace Qx.Common
{
    [Serializable]
    public class LiteModule 
    {
        public virtual int ID { private set; get; }

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
                RemoteObjectProvider.GetDictionaryAccess().SaveOrUpdateByName(Name, value, lang);
                ContentDictionary.SaveOrUpdateValue(Name, value, lang);
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
