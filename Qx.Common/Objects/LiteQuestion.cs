using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nachshon.Validation;

namespace Qx.Common
{
    [Serializable]
    public class LiteQuestion : ValidObjectWithIdentity
    {
        public virtual int ID { private set; get; }

        public virtual string Name { set; get; }

        public virtual QuestionType QuestionType { set; get; }

        public virtual bool IsDeleted { set; get; }

        public virtual string QuestionHebText
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

        public LiteQuestion()
        {
        }

        protected override object GetObjectId()
        {
            return ID;
        }
    }
}
