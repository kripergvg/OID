using System.Threading.Tasks;
using OID.DataProvider.Models;
using OID.DataProvider.Models.Deal;
using OID.DataProvider.Models.Deal.In;

namespace OID.DataProvider.Interfaces
{
    public interface IDealProvider
    {
        Task<DataProviderVoidModel> Approve(string dealId);

        Task<DataProviderVoidModel> Leave(string dealId);

        Task<DataProviderVoidModel> UpdateDelevery(DeleveryUpdateModel deleveryModel);

        Task<DataProviderModel<PaymentStatusModel>> ExecutePayment(ExecutePaymentModel paymentModel, string paymentStatus);
    }
}
