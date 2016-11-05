using OID.DataProvider.Models;

namespace OID.Web.Authenticate
{
    public interface IUserManager
    {
        void SetUser(UserModel userModel);

        UserModel GetUser();

        void UpdateSessionId(ISessionModel model);

        bool Authenticated();

        void RemoveUser();
    }
}
