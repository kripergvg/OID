using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using OID.Web.Models.User;

namespace OID.Web.Models.Partials
{
    public class AddressViewModel : PartialModel
    {
        public AddressViewModel() : base(null)
        {
            
        }

        public AddressViewModel(string modelPrefix) : base(modelPrefix)
        {
            City = new CityViewModel();
            Locality = new LocalityViewModel();
        }

        [Display(Name = "Ваш адрес для курьера")]
        public AddressType DeliveryLocationType { get; set; }

        [Display(Name = "Регион")]
        public string RegionCode { get; set; }

        public SelectList RegionList { get; set; }

        public CityViewModel City { get; set; }

        public LocalityViewModel Locality { get; set; }

        [Display(Name = "Адрес")]
        public string Address { get; set; }
    }
}
