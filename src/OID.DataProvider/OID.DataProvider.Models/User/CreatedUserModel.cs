namespace OID.DataProvider.Models.User
{
    public class CreatedUserModel: ISessionModel
    {
        public CreatedUserModel(string sessionId)
        {
            SessionId = sessionId;
        }

        public string SessionId { get; }
    }
}
