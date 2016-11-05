using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OID.Web.Models
{
    public class PhoneViewModel
    {
        public string Phone { get; set; }

        [Display(Name = "Комментарий")]
        public string Comment { get; set; }
    }
}
