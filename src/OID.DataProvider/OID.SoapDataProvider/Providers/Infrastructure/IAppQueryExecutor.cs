using System.Collections.Generic;
using System.Threading.Tasks;
using OID.SoapDataProvider.QuerySerializator;

namespace OID.SoapDataProvider.Providers.Infrastructure
{
    public interface IAppQueryExecutor
    {
        Task<QueryResult> Execute(List<Query> existedQueries);
    }
}
