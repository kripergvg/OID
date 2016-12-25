using System.ComponentModel.DataAnnotations;
using OID.Web.Models.Partials;

namespace OID.Web.Models.User
{
    public class ManageUserViewModel
    {

        [Display(Name = "Номер мобильного телефона")]
        public PhoneViewModel PhoneMobile { get; set; }

        [Display(Name = "Номер рабочего телефона")]
        public PhoneViewModel PhoneWork { get; set; }

        [Display(Name = "Номер домашнего телефона")]
        public PhoneViewModel PhoneHome { get; set; }

        [Display(Name = "Дополнительный номер телефона")]
        public PhoneViewModel PhoneAdditional { get; set; }

        public AddressViewModel AddressModel { get; set; }
    }
}
