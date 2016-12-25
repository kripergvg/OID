using System.ComponentModel.DataAnnotations;

namespace OID.Web.Models.User
{
    public class PhoneViewModel
    {
        public string Phone { get; set; }

        [Display(Name = "Комментарий")]
        public string Comment { get; set; }
    }
}
