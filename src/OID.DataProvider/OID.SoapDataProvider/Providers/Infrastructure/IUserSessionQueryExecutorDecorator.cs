using System.Collections.Generic;
using System.Threading.Tasks;

namespace OID.SoapDataProvider.Providers.Infrastructure
{
    public interface IUserSessionQueryExecutorDecorator
    {
        Task<SessionQueryResult> Execute(List<Query> existedQueries);
    }
}
