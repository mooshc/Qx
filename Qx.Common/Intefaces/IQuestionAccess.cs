
using System.Collections.Generic;

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
