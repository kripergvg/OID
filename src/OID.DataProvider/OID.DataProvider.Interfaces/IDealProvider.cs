using System.Threading.Tasks;
using OID.DataProvider.Models;
using OID.DataProvider.Models.Deal;
using OID.DataProvider.Models.Deal.In;

namespace OID.DataProvider.Interfaces
{
    public interface IDealProvider
    {
        Task<DataSessionProviderVoidModel> Approve(UserModel userModel, string dealId);

        Task<DataSessionProviderVoidModel> Leave(UserModel userModel, string dealId);

        Task<DataSessionProviderVoidModel> UpdateDelevery(UserModel userModel, DeleveryUpdateModel deleveryModel);

        Task<DataSessionProviderModel<PaymentStatusModel>> ExecutePayment(UserModel userModel, ExecutePaymentModel paymentModel, string paymentStatus);
    }
}
