using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OID.Web.Models
{
    public class ObjectModifyViewModel
    {
        public ObjectModifyViewModel()
        {
            ObjectChecks = new List<ObjectCheck>();
        }

        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        public SelectList CategoryList { get; set; }

        [Display(Name = "Категория объекта")]
        public string CategoryCode { get; set; }

        [Display(Name = "Статус объекта")]
        public ObjectStatusType ObjectStatusType { get; set; }

        public IList<ObjectCheck> ObjectChecks { get; set; }

        public class ObjectCheck
        {
            public string CheckId { get; set; }

            [Display(Name = "Вид проверки")]
            public CheckType CheckType { get; set; }

            [Display(Name = "Подробное описание проверки")]
            public string Description { get; set; }

            [Display(Name = "Фотография для проверки")]
            public string ImageUrl { get; set; }
        }
    }
}
