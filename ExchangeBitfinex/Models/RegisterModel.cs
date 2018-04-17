using System.ComponentModel.DataAnnotations;

namespace ExchangeBitfinex.Models
{
    /// <summary>
    /// Данные для регистрации.
    /// </summary>
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
