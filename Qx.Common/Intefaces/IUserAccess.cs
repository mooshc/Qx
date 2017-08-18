using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nachshon.ObjectAccess;

namespace Qx.Common
{
    public interface IUserAccess : IObjectAccess<User>
    {
        User IsLoginCorrect(string username, string password);
        bool IsGuidCorrect(Guid guid);
        bool IsAdminUser(Guid guid);
        void PermitUserToAnamnesis(User user, Module anamnesis);
    }
}
