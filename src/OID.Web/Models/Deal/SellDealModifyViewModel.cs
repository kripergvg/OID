using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OID.Web.Models.Deal
{
    public class SellDealModifyViewModel: DealModifyModel
    {
        public SelectList UserAccounts { get; set; }

        [Display(Name = "Номер счета")]
        public int? UserAccountId { get; set; }
    }
}
