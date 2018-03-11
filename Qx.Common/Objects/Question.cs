using Qx.Common.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qx.Common
{
    [Serializable]
    public class Question : TranslatedObject
    {
        public virtual int ID { set; get; }

        public virtual string Name { set; get; }

        public virtual bool IsEnter { set; get; }
        
        public virtual string EndingChar { set; get; }

        public virtual bool IsWithoutEndingChar { set; get; }

        public virtual QuestionType QuestionType { set; get; }

        public virtual bool IsMandatory { set; get; }

        public virtual bool IsDeleted { set; get; }

        public virtual IList<Answer> Answers { set; get; }

        public virtual IList<Combination> Combinations { set; get; }

        public virtual string QuestionHebText
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

        public virtual string PreQuestionHebText
        {
            set
            {
                var lang = CommonFunctions.HebLang;
                SaveOrUpdateByName("Pre_" + Name, value, lang);
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
                SaveOrUpdateByName("Pre_" + Name + "-Female", value, lang);
            }
            get
            {
                return ContentDictionary.GetContent("Pre_" + Name + "-Female", CommonFunctions.HebLang);
            }
        }

        public virtual string ToolTipHebText
        {
            set
            {
                var lang = CommonFunctions.HebLang;
                SaveOrUpdateByName(Name + "_ToolTip", value, lang);
            }
            get
            {
                return ContentDictionary.GetContent(Name + "_ToolTip", CommonFunctions.HebLang);
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

        public virtual void ZeroID()
        {
            ID = 0;
        }
    }
}
