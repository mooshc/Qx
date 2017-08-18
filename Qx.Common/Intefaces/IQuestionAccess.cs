using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nachshon.ObjectAccess;

namespace Qx.Common
{
    public interface IQuestionAccess : IObjectAccess<Question>
    {
        List<string> GetAllQuestionsNames();
        Question LoadByName(string name);
        List<QuestionInModule> LoadPermanentQuestions();
        List<Question> GetQuestionWorkpool();
        void LoadPermanentQuestionsToServer();
    }
}
