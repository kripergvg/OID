using System.Collections.Generic;
using System.Threading.Tasks;
using OID.DataProvider.Models;
using OID.DataProvider.Models.Object;
using OID.DataProvider.Models.Object.In;

namespace OID.DataProvider.Interfaces
{
    public interface IDealObjectProvider
    {
        Task<DataSessionProviderModel<UpsertObjectModel>> Upsert(UserModel userModel, DealObject dealObject);

        Task<DataSessionProviderVoidModel> Approve(UserModel userModel, string dealId);

        Task<DataSessionProviderModel<List<UserObject>>> GetUserObjects(UserModel userModel);
    }
}
