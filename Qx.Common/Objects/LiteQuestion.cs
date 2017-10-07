﻿using System;

namespace Qx.Common
{
    [Serializable]
    public class LiteQuestion
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
    }
}
