using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OID.Web.Models.Deal
{
    public class UpdateSellDealViewModel
    {
        public int DealId { get; set; }

        public SellDealModifyViewModel SellDealModel { get; set; }
    }
}
