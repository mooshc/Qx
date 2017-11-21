using System;

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
