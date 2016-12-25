using System.ComponentModel.DataAnnotations;

namespace OID.Web.Models.Object
{
    public enum CheckType
    {
        [Display(Name = "Проверка функций")]
        Function = 1,

        [Display(Name = "Проверка состояния")]
        Condition = 2,

        [Display(Name = "Проверка комплектации")]
        Equipment = 3,

        [Display(Name = "Нестандартная проверка")]
        Custom = 4
    }
}
