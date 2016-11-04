namespace OID.DataProvider.Models
{
    public class UserModel
    {
        public UserModel(string login, string passwordHash, string sessionId)
        {
            Login = login;
            PasswordHash = passwordHash;
            SessionId = sessionId;
        }

        public string Login { get; set; }

        public string PasswordHash { get; set; }

        public string SessionId { get; set; }
    }
}
