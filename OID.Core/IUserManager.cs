namespace OID.Core
{
    public interface IUserManager
    {
        void SetUser(UserModel userModel);

        UserModel GetUser();

        void UpdateSessionId(string sessionId);

        bool Authenticated();

        void RemoveUser();
    }
}
