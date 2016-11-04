using System.Threading.Tasks;
using OID.DataProvider.Models;
using OID.DataProvider.Models.Deal;
using OID.DataProvider.Models.Deal.In;

namespace OID.DataProvider.Interfaces
{
    public interface IDealProvider
    {
        Task<DataProviderModel<SessionModel>> Approve(UserModel userModel, string dealId);

        Task<DataProviderModel<SessionModel>> Leave(UserModel userModel, string dealId);

        Task<DataProviderModel<SessionModel>> UpdateDelevery(UserModel userModel, DeleveryUpdateModel deleveryModel);

        Task<DataProviderModel<PaymentStatusModel>> ExecutePayment(UserModel userModel, ExecutePaymentModel paymentModel, string paymentStatus);
    }
}
