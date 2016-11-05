using System.Collections.Generic;
using System.Threading.Tasks;
using OID.SoapDataProvider.QuerySerializator;
using OID.SoapDataProvider.SoapServiceClient;

namespace OID.SoapDataProvider.Providers.Infrastructure
{
    public class QueryExecutor : IQueryExecutor
    {
        private readonly IQuerySerializator _querySerializator;
        private readonly ISoapServiceClient _serviceClient;

        public QueryExecutor(IQuerySerializator querySerializator, ISoapServiceClient serviceClient)
        {
            _querySerializator = querySerializator;
            _serviceClient = serviceClient;
        }

        public async Task<QueryResult> Execute(List<Query> existedQueries)
        {
            var serializedQueries = _querySerializator.ToStringXml(existedQueries);
            var response = await _serviceClient.PostAsync(serializedQueries).ConfigureAwait(false);
            var queries = _querySerializator.ToQueryList(response);

            return queries;
        }
    }
}
