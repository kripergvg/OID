using System;

namespace OID.DataProvider.Models.Deal.In
{
    public class ExecutePaymentModel
    {
        public ExecutePaymentModel(DateTime? date, string transactionNumber, string billNumber, string paymentId, string cptyServiceId, string info, string comment, string password)
        {
            Date = date;
            TransactionNumber = transactionNumber;
            BillNumber = billNumber;
            PaymentId = paymentId;
            CptyServiceId = cptyServiceId;
            Info = info;
            Comment = comment;
            Password = password;
        }

        public DateTime? Date { get; }

        public string TransactionNumber { get; }

        public string BillNumber { get; }

        public string PaymentId { get; }

        public string CptyServiceId { get; }

        public string Info { get; }

        public string Comment { get; }

        public string Password { get; }
    }
}
