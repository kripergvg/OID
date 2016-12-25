using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using OID.DataProvider.Models.Deal;
using OID.DataProvider.Models.User;
using OID.Web.Models.Partials;

namespace OID.Web.Models.Deal
{
    public class SellDealModifyViewModel
    {
        public SellDealModifyViewModel()
        {
            SelectedDealObjects = new List<SelectedDealObject>();
        }

        public SelectList FreeDealObjects { get; set; }

        public IList<SelectedDealObject> SelectedDealObjects { get; set; }

        [Display(Name = "Цена")]
        public double Price { get; set; }

        [Display(Name = "Комментарий")]
        public string Comment { get; set; }

        public SelectList DeleveryTypes { get; set; }

        [Display(Name = "Тип доставки")]
        public int DeleveryTypeId { get; set; }

        [Display(Name = "Вес")]
        public int Weight { get; set; }

        public Size Size { get; set; }

        public AddressViewModel AddressModel { get; set; }

        public SelectList UserAccounts { get; set; }

        [Display(Name = "Номер счета")]
        public int? UserAccountId { get; set; }

        [Display(Name = "Тип счета")]
        public PaymentType PaymentType { get; set; }

        [Display(Name = "Номер счета")]
        public int? AccountNumber { get; set; }

        public AccountAction AccountAction { get; set; } = AccountAction.New;

        public class SelectedDealObject
        {
            public int ObjectId { get; set; }

            public string Name { get; set; }
        }
    }
}
