using OID.SoapDataProvider.QuerySerializator;

namespace OID.SoapDataProvider
{
    public class SessionQueryResult : QueryResult
    {
        public SessionQueryResult(QueryResult result, string sessionId)
            : base(result.ResultMessage, result.Queries)
        {
            SessionId = sessionId;
        }

        public string SessionId { get; set; }
    }
}
