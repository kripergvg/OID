using OID.DataProvider.Models.Deal;

namespace OID.SoapDataProvider
{
    public interface ISoapParser
    {
        bool BoolParse(object value);

        PaymentStatus ParsePaymentStatus(object value);

        PaymentOperation ParsePaymentOperation(object value);

        DealService ParseDealService(object value);

        CheckStatus ParseCheckStatus(object value);

        CheckType ParseCheckType(object value);
    }
}
