using System.ComponentModel.DataAnnotations;

namespace ExchangeBitfinex.Models
{
    /// <summary>
    /// Данные для регистрации.
    /// </summary>
    public class RegisterModel
    {
        /// <summary>
        /// Email (учетная запись).
        /// </summary>
        [EmailAddress(ErrorMessage = "Необходимо указать корректный адрес электронной почты.")]
        [Required(ErrorMessage = "Необходимо указать адрес электронной почты.")]
        public string Email { get; set; }

        /// <summary>
        /// Пароль.
        /// </summary>
        [Required]
        [DataType(DataType.Password, ErrorMessage = "Необходимо указать пароль.")]
        public string Password { get; set; }
    }
}
