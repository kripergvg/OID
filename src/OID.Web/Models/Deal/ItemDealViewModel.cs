using System;
using OID.DataProvider.Models.Deal;

namespace OID.Web.Models.Deal
{
    public class ItemDealViewModel
    {
        public string Objects { get; set; }

        public double Price { get; set; }

        public string Comment { get; set; }

        public int DealId { get; set; }

        public int Pin { get; set; }

        public DealType DealType { get; set; }

        public bool Blocked { get; set; }

        public bool IsMyDeal { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
