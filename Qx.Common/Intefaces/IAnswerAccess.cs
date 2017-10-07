using System.Collections.Generic;

namespace Qx.Common
{
    public interface IAnswerAccess : IObjectAccess<Answer>
    {
        string GetAnswerName(int id);
        List<string> GetAllNames();
        int GetAnswerID(string name);
    }
}
