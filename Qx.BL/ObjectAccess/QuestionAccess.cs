using System.Collections.Generic;
using System.Linq;
using Qx.Common;
using NHibernate.Context;
using NHibernate.Linq;
using System.Data;

namespace Qx.BL
{
    public class QuestionAccess : ObjectAccessNHibernate<Question> , IQuestionAccess
    {
        public QuestionAccess(ICurrentSessionContext sessionContext)
            : base(sessionContext)
        {
        }

        public List<string> GetAllQuestionsNames()
        {
            var s = sessionContext.CurrentSession();
            var result = s.CreateSQLQuery("call `GetAllQuestionsNames`();");
            return result.List<string>().ToList();
        }

        public Question LoadByName(string name)
        {
            var s = sessionContext.CurrentSession();

            return s.Linq<Question>().Where(q => q.Name == name).ToList().FirstOrDefault();
        }

        public List<QuestionInModule> LoadPermanentQuestions()
        {
            if (ServerStatics.PermanentQuestions == null)
            {
                var s = sessionContext.CurrentSession();
                var list = s.Linq<Question>().Where(q => /*q.Name == "GeneralImpression" || q.Name == "h_SkinColor" || q.Name == "e_OtherFindings" ||*/ q.Name == "h_OtherRemarks").ToList();
                return new List<QuestionInModule>() { 
                //new QuestionInModule(list.Where(q => q.Name == "GeneralImpression").First() , 0, false), 
                //new QuestionInModule(list.Where(q => q.Name == "h_SkinColor").First(), 0, false), 
                //new QuestionInModule(list.Where(q => q.Name == "e_OtherFindings").First(), 1000, false), 
                new QuestionInModule(list.Where(q => q.Name == "h_OtherRemarks").First(), 1000, false) };
            }
            return ServerStatics.PermanentQuestions;
        }

        public void LoadPermanentQuestionsToServer()
        {
            var s = sessionContext.CurrentSession();
            var list = s.Linq<Question>().Where(q => /*q.Name == "GeneralImpression" || q.Name == "SkinColor" || q.Name == "e_OtherFindings" ||*/ q.Name == "h_OtherRemarks").ToList();
            ServerStatics.PermanentQuestions = new List<QuestionInModule>() { 
                //new QuestionInModule(list.Where(q => q.Name == "GeneralImpression").First() , 0, false), 
                //new QuestionInModule(list.Where(q => q.Name == "SkinColor").First(), 0, false), 
                //new QuestionInModule(list.Where(q => q.Name == "e_OtherFindings").First(), 1000, false), 
                new QuestionInModule(list.Where(q => q.Name == "h_OtherRemarks").First(), 1000, false) };
        }

        public List<Question> GetQuestionWorkpool()
        {
            var s = sessionContext.CurrentSession();
            var result = s.CreateSQLQuery("call `LiteQuestions`();");
            
            var x = result.List();
            return null;
        }

        /*public override IEnumerable<Question> SaveOrUpdate(IEnumerable<Question> objs)
        {
            var hebLang = ServiceLocator.LanguageAccess.Load(1);
            foreach (Question qu in objs)
            {
                foreach (Answer ans in qu.Answers)
                {
                    ServiceLocator.DictionaryAccess.SaveOrUpdateByName(ans.Name, ans.AnswerHebText, hebLang);
                    ServiceLocator.DictionaryAccess.SaveOrUpdateByName(ans.Name + "_Male", ans.ResultMaleHebText, hebLang);
                    ServiceLocator.DictionaryAccess.SaveOrUpdateByName(ans.Name + "_Female", ans.ResultFemaleHebText, hebLang);
                }
            }
            return base.SaveOrUpdate(objs);
        }*/
    }
}
