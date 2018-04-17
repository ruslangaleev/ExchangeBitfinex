namespace ExchangeBitfinex.Models
{
    /// <summary>
    /// Модель для отображения учетной записи.
    /// </summary>
    public class ApplicationUserViewModel
    {
        /// <summary>
        /// ФИО.
        /// </summary>
        public string Fullname { get; set; }

        /// <summary>
        /// Телефон.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Email.
        /// </summary>
        public string Email { get; set; }
    }
}
