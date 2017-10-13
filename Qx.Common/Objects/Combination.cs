using Qx.Common.Objects;
using System;
using System.Collections.Generic;

namespace Qx.Common
{
    [Serializable]
    public class Combination : TranslatedObject
    {
        public virtual int ID { private set; get; }

        public virtual Module Module { set; get; }

        public virtual Question Question { set; get; }

        public virtual Decimal Order { set; get; }

        public virtual string Name { set; get; }

        public virtual bool IsDeleted { set; get; }

        public virtual IList<CombinatedAnswer> CombinatedAnswers { set; get; }

        public virtual bool IsExisting { set; get; }

        /*public virtual string CombinationHebText
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
        }*/

        public virtual string ResultMaleHebText
        {
            set
            {
                var lang = CommonFunctions.HebLang;
                SaveOrUpdateByName(Name + "_Male", value, lang);
            }
            get
            {
                return ContentDictionary.GetContent(Name + "_Male", CommonFunctions.HebLang);
            }
        }

        public virtual string ResultFemaleHebText
        {
            set
            {
                var lang = CommonFunctions.HebLang;
                SaveOrUpdateByName(Name + "_Female", value, lang);
            }
            get
            {
                return ContentDictionary.GetContent(Name + "_Female", CommonFunctions.HebLang);
            }
        }

        public Combination()
        {
            CombinatedAnswers = new List<CombinatedAnswer>();
            IsExisting = false;
        }

        public override bool Equals(object obj)
        {
            return obj is Combination && (obj as Combination).Name == Name;
        }
    }
}
