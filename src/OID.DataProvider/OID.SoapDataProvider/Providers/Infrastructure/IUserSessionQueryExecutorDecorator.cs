using System.Collections.Generic;
using System.Threading.Tasks;
using OID.DataProvider.Models;

namespace OID.SoapDataProvider.Providers.Infrastructure
{
    public interface IUserSessionQueryExecutorDecorator
    {
        Task<SessionQueryResult> Execute(List<Query> existedQueries, UserModel userModel);
    }
}
