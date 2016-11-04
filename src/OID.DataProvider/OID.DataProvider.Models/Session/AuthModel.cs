namespace OID.DataProvider.Models.Session
{
    public class AuthModel
    {
        public AuthModel(string userName, string sessionId)
        {
            UserName = userName;
            SessionId = sessionId;
        }

        public string UserName { get; }

        public string SessionId { get; set; }
    }
}
