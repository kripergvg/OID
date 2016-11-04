namespace OID.DataProvider.Models.Object
{
    public class UpsertObjectModel : ISessionModel
    {
        public UpsertObjectModel(string objectId, string sessionId)
        {
            ObjectId = objectId;
            SessionId = sessionId;
        }

        public string ObjectId { get; }

        public string SessionId { get; }
    }
}
