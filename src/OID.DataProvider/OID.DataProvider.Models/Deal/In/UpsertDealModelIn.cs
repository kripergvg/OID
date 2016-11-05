using System.Collections.Generic;
using OID.DataProvider.Models.Object.In;

namespace OID.DataProvider.Models.Deal.In
{
    public class UpsertDealModelIn
    {
        public string BuySell { get; set; }

        public string Price { get; set; }

        public string Comment { get; set; }

        public string DealId { get; set; }

        public Account UserAccount { get; set; }

        public List<DealObject> ObjectList { get; set; }

        public class Account
        {
            public string AccountId { get; set; }

            public bool Deleted { get; set; }

            public string UserAccountId { get; set; }

            public string AccountNumber { get; set; }

            public string PaymentCptyServiceId { get; set; }
        }
    }
}
