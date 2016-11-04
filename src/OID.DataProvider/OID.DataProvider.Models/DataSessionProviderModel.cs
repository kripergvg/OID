namespace OID.DataProvider.Models
{
    public class DataSessionProviderModel<T> : DataProviderModel<T>, ISessionModel
    {
        public DataSessionProviderModel(ResultMessage resultMessage, T model, string sessionId) 
            : base(resultMessage, model)
        {
            SessionId = sessionId;
        }

        public string SessionId { get; }
    }
}
