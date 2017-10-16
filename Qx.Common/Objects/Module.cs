using Qx.Common.Objects;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Qx.Common
{
    [Serializable]
    public class Module : TranslatedObject
    {
        public virtual int ID { set; get; }

        public virtual string Name { set; get; }

        public virtual ModuleType ModuleType { set; get; }

        public virtual string GroupName { set; get; }

        public virtual int SeverityLevel { set; get; }

        public virtual string Tags { set; get; }

        public virtual bool? IsMale { set; get; }

        public virtual bool IsDeleted { set; get; }

        public virtual IList<QuestionInModule> Questions { set; get; }

        public virtual IList<PhysicalExaminationInAnamnesis> PhysicalExaminations { set; get; }

        public virtual IList<Combination> Combinations { set; get; }

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
        
        public Module()
        {
            Questions = new List<QuestionInModule>();
            PhysicalExaminations = new List<PhysicalExaminationInAnamnesis>();
            Combinations = new List<Combination>();
        }

        public Module(int id) : this()
        {
            ID = id;
        }

        public virtual IList<Answer> GetAllAnswers()
        {
            var answers = new List<Answer>();
            foreach (var question in Questions)
                answers.AddRange(question.Question.Answers);
            return answers;
        }
    }
}
