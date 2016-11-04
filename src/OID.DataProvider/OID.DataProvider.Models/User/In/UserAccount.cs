namespace OID.DataProvider.Models.User.In
{
    public class Account
    {
        public Account(string userAccountId, string accountId, string accountNumber, string paymentCptyServiceId, bool deleted)
        {
            UserAccountId = userAccountId;
            AccountId = accountId;
            AccountNumber = accountNumber;
            PaymentCptyServiceId = paymentCptyServiceId;
            Deleted = deleted;
        }

        public string UserAccountId { get; }

        public string AccountId { get; }

        public string AccountNumber { get; }

        public string PaymentCptyServiceId { get; }

        public bool Deleted { get; }
    }
}
