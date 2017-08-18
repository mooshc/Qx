using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nachshon.ObjectAccess;

namespace Qx.Common
{
    public interface IAnswerAccess : IObjectAccess<Answer>
    {
        string GetAnswerName(int id);
        List<string> GetAllNames();
        int GetAnswerID(string name);
    }
}
