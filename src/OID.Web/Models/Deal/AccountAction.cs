using System.ComponentModel.DataAnnotations;

namespace OID.Web.Models.Deal
{
    public enum AccountAction
    {
        [Display(Name = "Существующий")]
        Existed = 1,

        [Display(Name = "Новый")]
        New = 2
    }
}
