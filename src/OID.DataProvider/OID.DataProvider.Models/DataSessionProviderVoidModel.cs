namespace OID.DataProvider.Models
{
    public class DataSessionProviderVoidModel : DataProviderVoidModel, ISessionModel
    {
        public DataSessionProviderVoidModel(ResultMessage resultMessage,string sessionId) 
            : base(resultMessage)
        {
            SessionId = sessionId;
        }

        public string SessionId { get; }
    }
}
