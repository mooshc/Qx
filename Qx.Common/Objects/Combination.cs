using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nachshon.Validation;

namespace Qx.Common
{
    [Serializable]
    public class Combination : ValidObjectWithIdentity
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
                RemoteObjectProvider.GetDictionaryAccess().SaveOrUpdateByName(Name + "_Male", value, lang);
                ContentDictionary.SaveOrUpdateValue(Name + "_Male", value, lang);
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
                RemoteObjectProvider.GetDictionaryAccess().SaveOrUpdateByName(Name + "_Female", value, lang);
                ContentDictionary.SaveOrUpdateValue(Name + "_Female", value, lang);
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

        protected override object GetObjectId()
        {
            return ID;
        }

        public override bool Equals(object obj)
        {
            return obj is Combination && (obj as Combination).Name == Name;
        }
    }
}
