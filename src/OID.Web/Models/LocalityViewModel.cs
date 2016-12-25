using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OID.Web.Models
{
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
