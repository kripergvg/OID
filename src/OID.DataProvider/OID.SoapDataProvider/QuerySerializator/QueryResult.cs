using System.Collections.Generic;
using OID.DataProvider.Models;

namespace OID.SoapDataProvider.QuerySerializator
{
    public class QueryResult
    {
        public QueryResult(ResultMessage resultMessage, List<Query> queries)
        {
            ResultMessage = resultMessage;
            Queries = queries;
        }

        public ResultMessage ResultMessage { get; }

        public List<Query> Queries { get; }
    }
}
