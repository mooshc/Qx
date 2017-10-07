using System;
using System.Collections.Generic;

namespace Qx.Common
{
    [Serializable]
    public class Answer
    {
        public virtual int ID { private set; get; }

        public virtual string Name { set; get; }

        public virtual Question Question { set; get; }

        public virtual Question ExtraQuestion { set; get; }

        public virtual string ImageFileName { set; get; }

        public virtual bool IsContainsTextBox { set; get; }

        public virtual bool IsTextBoxDigitsOnly { set; get; }

        public virtual bool IsSingular { set; get; }

        public virtual Module RecomendedPhysicalEx { set; get; }

        public virtual bool IsDeleted { set; get; }

        public virtual IList<Condition> WarningConditions { set; get; }

        public virtual DateTime TimeStamp { set; get; }

        public virtual string AnswerHebText
        {
            set
            {
                var lang = CommonFunctions.HebLang;
                RemoteObjectProvider.GetDictionaryAccess().SaveOrUpdateByName(Name, value, lang);
                ContentDictionary.SaveOrUpdateValue(Name, value, lang);
                //_AnswerHebText = value;
            }
            get
            {
                return ContentDictionary.GetContent(Name, CommonFunctions.HebLang);
            }
        }

        public virtual string ResultMaleHebText
        {
            set
            {
                var lang = CommonFunctions.HebLang;
                RemoteObjectProvider.GetDictionaryAccess().SaveOrUpdateByName(Name + "_Male", value, lang);
                ContentDictionary.SaveOrUpdateValue(Name + "_Male", value, lang);
                //_ResultMaleHebText = value;
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
                //_ResultFemaleHebText = value;
            }
            get
            {
                return ContentDictionary.GetContent(Name + "_Female", CommonFunctions.HebLang);
            }
        }

        //private string _AnswerHebText;
        //private string _ResultMaleHebText;
        //private string _ResultFemaleHebText;

        public Answer()
        {
            WarningConditions = new List<Condition>();
        }

        public override bool Equals(object obj)
        {
            return obj is Answer && (obj as Answer).Name == Name;
        }

        public virtual void ZeroID()
        {
            ID = 0;
        }
    }
}
