using System.Collections.Generic;
using System.Threading.Tasks;
using OID.DataProvider.Models;
using OID.DataProvider.Models.Deal;
using OID.DataProvider.Models.Object;
using OID.DataProvider.Models.Object.In;

namespace OID.DataProvider.Interfaces
{
    public interface IDealObjectProvider
    {
        //Task<DataProviderModel<UpsertObjectModel>> Upsert(DealObject dealObject);

        Task<DataProviderVoidModel> Approve(string dealId);

        Task<DataProviderModel<List<UserObject>>> GetUserObjects(bool onlyNotBlocked = false, DealType? dealType = null);

        Task<DataProviderModel<List<ObjectCategory>>> GetCategories();

        Task<DataProviderModel<string>> CreateObject(CreateDealObject dealObject);

        Task<DataProviderModel<List<CheckListItem>>> GetChecks(string checkListId);

        Task<DataProviderModel<UserObject>> GetUserObject(string objectID);

        Task<DataProviderVoidModel> DeleteChecks(List<string> checkIds, string checkListId, string objectId);

        Task<DataProviderVoidModel> CreateChecks(List<DealCheck> checks, string checkListId, string objectId);

        Task<DataProviderVoidModel> UpdateObject(UpdateDealObject dealObject);

        Task<DataProviderVoidModel> DeleteObject(string objectId);

        Task<DataProviderModel<List<DealObject>>> GetDealObjects(int dealId);
    }
}
