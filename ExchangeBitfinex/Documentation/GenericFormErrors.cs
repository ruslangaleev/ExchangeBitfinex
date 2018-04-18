namespace ExchangeBitfinex.Documentation
{
    /// <summary>
    /// Модель для API документации (не используется в приложении)
    /// </summary>
    public class GenericFormErrors
    {
        /// <summary>
        /// Массив ошибок по указанному полю
        /// </summary>
        public string[] FieldName1 { get; set; }

        /// <summary>
        /// Массив ошибок по указанному полю
        /// </summary>
        public string[] FieldNameN { get; set; }

        /// <summary>
        /// Массив ошибок общего характера
        /// </summary>
        public string[] _global { get; set; }
    }
}
