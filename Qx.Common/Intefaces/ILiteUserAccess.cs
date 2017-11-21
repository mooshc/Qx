
namespace Qx.Common
{
    public interface ILiteUserAccess : IObjectAccess<LiteUser>
    {
        LiteUser IsLoginCorrect(string username, string password);
    }
}
