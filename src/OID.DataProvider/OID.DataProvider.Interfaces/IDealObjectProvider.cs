using System.Collections.Generic;
using System.Threading.Tasks;
using OID.DataProvider.Models;
using OID.DataProvider.Models.Object;
using OID.DataProvider.Models.Object.In;
using OID.DataProvider.Models.User;

namespace OID.DataProvider.Interfaces
{
    public interface IDealObjectProvider
    {
        Task<DataProviderModel<UpsertObjectModel>> Upsert(DealObject dealObject);

        Task<DataProviderVoidModel> Approve(string dealId);

        Task<DataProviderModel<List<UserObject>>> GetUserObjects();
    }
}
