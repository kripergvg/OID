using System;

namespace OID.DataProvider.Models.User
{
    public class UserAccount
    {
        public UserAccount(int userAccountId, int accountId, PaymentType paymentService, int accountNumber, DateTime createDate)
        {
            UserAccountId = userAccountId;
            AccountId = accountId;
            PaymentService = paymentService;
            AccountNumber = accountNumber;
            CreateDate = createDate;
        }

        public int UserAccountId { get; set; }

        public int AccountId { get; set; }

        public PaymentType PaymentService { get; set; }

        public int AccountNumber { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
