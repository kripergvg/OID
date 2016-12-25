using System.ComponentModel.DataAnnotations;

namespace OID.Web.Models.User
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Поле '{0}' должно быть заполнено")]
        [StringLength(200, ErrorMessage = "Поле '{0}' не должно превышать {1} символов")]
        [Display(Name = "Отображаемое имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле '{0}' должно быть заполнено")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Поле '{0}' содержит некорректный адрес электронной почты")]
        [DataType(DataType.EmailAddress)]
        [StringLength(200, ErrorMessage = "Поле '{0}' не должно превышать {1} символов")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле '{0}' должно быть заполнено")]
        [StringLength(1000, ErrorMessage = "Поле '{0}' не должно быть менее {2} символов", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Введенные пароли должны совпадать")]
        public string ConfirmPassword { get; set; }
    }
}
