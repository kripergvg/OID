using System.Threading.Tasks;
using OID.DataProvider.Models;
using OID.DataProvider.Models.Session;

namespace OID.DataProvider.Interfaces
{
    public interface ISessionProvider
    {
        Task<DataProviderModel<AuthModel>> Authenticate(string email, string passwordHash);        

        Task<DataProviderVoidModel> CloseSession(string sessionId);
    }
}
