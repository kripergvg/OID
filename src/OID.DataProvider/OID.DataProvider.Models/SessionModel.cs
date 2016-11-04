namespace OID.DataProvider.Models
{
    public class SessionModel : ISessionModel
    {
        public SessionModel(string sessionId)
        {
            SessionId = sessionId;
        }

        public string SessionId { get; }
    }
}
