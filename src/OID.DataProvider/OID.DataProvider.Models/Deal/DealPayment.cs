using System;

namespace OID.DataProvider.Models.Deal
{
    public class DealPayment
    {
        public DealPayment(int dealPaymentId, int? paymentDealPaymentId, bool isParentDealExecuted, Direction direction, int paymentId, int? sourcePaymentId,
            int? cptyServiceId, double amount, DateTime? date, string transactionNum, PaymentStatus paymentStatus, PaymentOperation paymentOperation,
            DealService dealService, string currencyCode, DateTime createDate, bool isMyPayment, string accountNumberFrom, string accountNumberTo)
        {
            DealPaymentId = dealPaymentId;
            PaymentDealPaymentId = paymentDealPaymentId;
            IsParentDealExecuted = isParentDealExecuted;
            Direction = direction;
            PaymentId = paymentId;
            SourcePaymentId = sourcePaymentId;
            CptyServiceId = cptyServiceId;
            Amount = amount;
            Date = date;
            TransactionNum = transactionNum;
            PaymentStatus = paymentStatus;
            PaymentOperation = paymentOperation;
            DealService = dealService;
            CurrencyCode = currencyCode;
            CreateDate = createDate;
            IsMyPayment = isMyPayment;
            AccountNumberFrom = accountNumberFrom;
            AccountNumberTo = accountNumberTo;
        }

        public int DealPaymentId { get; set; }

        public int? PaymentDealPaymentId { get; set; }

        public bool IsParentDealExecuted { get; set; }

        public Direction Direction { get; set; }

        public int PaymentId { get; set; }

        public int? SourcePaymentId { get; set; }

        public int? CptyServiceId { get; set; }

        public double Amount { get; set; }

        public DateTime? Date { get; set; }

        public string TransactionNum { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public PaymentOperation PaymentOperation { get; set; }

        public DealService DealService { get; set; }

        public string CurrencyCode { get; set; }

        public DateTime CreateDate { get; set; }

        public bool IsMyPayment { get; set; }

        public string AccountNumberFrom { get; set; }

        public string AccountNumberTo { get; set; }
    }
}