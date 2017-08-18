﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nachshon.Validation;

namespace Qx.Common
{
    [Serializable]
    public class Question : ValidObjectWithIdentity
    {
        public virtual int ID { private set; get; }

        public virtual string Name { set; get; }

        public virtual bool IsEnter { set; get; }
        
        public virtual string EndingChar { set; get; }

        public virtual bool IsWithoutEndingChar { set; get; }

        public virtual QuestionType QuestionType { set; get; }

        public virtual bool IsDeleted { set; get; }

        public virtual IList<Answer> Answers { set; get; }

        public virtual IList<Combination> Combinations { set; get; }

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

        public virtual string PreQuestionHebText
        {
            set
            {
                var lang = CommonFunctions.HebLang;
                RemoteObjectProvider.GetDictionaryAccess().SaveOrUpdateByName("Pre_" + Name, value, lang);
                ContentDictionary.SaveOrUpdateValue("Pre_" + Name, value, lang);
            }
            get
            {
                return ContentDictionary.GetContent("Pre_" + Name, CommonFunctions.HebLang);
            }
        }

        public virtual string PreQuestionHebTextFemale
        {
            set
            {
                var lang = CommonFunctions.HebLang;
                RemoteObjectProvider.GetDictionaryAccess().SaveOrUpdateByName("Pre_" + Name + "-Female", value, lang);
                ContentDictionary.SaveOrUpdateValue("Pre_" + Name + "-Female", value, lang);
            }
            get
            {
                return ContentDictionary.GetContent("Pre_" + Name + "-Female", CommonFunctions.HebLang);
            }
        }

        public virtual int CalculatedHight
        {
            get
            { return 55 + (Answers.Where(a => !a.IsDeleted && !a.Name.Contains("#false")).Count() - 1) * 25; }
        }

        public Question()
        {
            Answers = new List<Answer>();
            Combinations = new List<Combination>();
        }

        protected override object GetObjectId()
        {
            return ID;
        }

        public virtual void ZeroID()
        {
            ID = 0;
        }
    }
}
