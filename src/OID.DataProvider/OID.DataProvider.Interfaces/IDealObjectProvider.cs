using System.Threading.Tasks;
using OID.DataProvider.Models;
using OID.DataProvider.Models.Object;
using OID.DataProvider.Models.Object.In;

namespace OID.DataProvider.Interfaces
{
    public interface IDealObjectProvider
    {
        Task<DataProviderModel<UpsertObjectModel>> Upsert(UserModel userModel, DealObject dealObject);
    }
}
