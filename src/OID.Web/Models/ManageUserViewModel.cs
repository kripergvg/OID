using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OID.Web.Models
{
    public class ManageUserViewModel
    {
        public ManageUserViewModel()
        {
            City = new CityViewModel();
            Locality = new LocalityViewModel();
        }

        [Display(Name = "Номер мобильного телефона")]
        public PhoneViewModel PhoneMobile { get; set; }

        [Display(Name = "Номер рабочего телефона")]
        public PhoneViewModel PhoneWork { get; set; }

        [Display(Name = "Номер домашнего телефона")]
        public PhoneViewModel PhoneHome { get; set; }

        [Display(Name = "Дополнительный номер телефона")]
        public PhoneViewModel PhoneAdditional { get; set; }

        [Display(Name = "Ваш адрес для курьера")]
        public AddressType DeliveryLocationType { get; set; }

        [Display(Name = "Регион")]
        public string RegionCode { get; set; }

        public SelectList RegionList { get; set; }

        public CityViewModel City { get; set; }

        public LocalityViewModel Locality { get; set; }

        [Display(Name = "Адрес")]
        public string Address { get; set; }

        public class CityViewModel
        {
            public CityViewModel()
            {
                CityList = new SelectList(Enumerable.Empty<object>());
            }

            [Display(Name = "Город")]
            public string CityCode { get; set; }

            public SelectList CityList { get; set; }
        }

        public class LocalityViewModel
        {
            public LocalityViewModel()
            {
                LocalityList = new SelectList(Enumerable.Empty<object>());
                LocationList = new SelectList(Enumerable.Empty<object>());
            }

            [Display(Name = "Район")]
            public string LocalityCode { get; set; }

            public SelectList LocalityList { get; set; }

            [Display(Name = "Населенный пункт")]
            public string LocationCode { get; set; }

            public SelectList LocationList { get; set; }
        }
    }
}
