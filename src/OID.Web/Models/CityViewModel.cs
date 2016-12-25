using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OID.Web.Models
{
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
}
