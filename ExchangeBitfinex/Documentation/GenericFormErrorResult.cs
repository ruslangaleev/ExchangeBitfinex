namespace ExchangeBitfinex.Documentation
{
    /// <summary>
    /// Общий ответ c ошибками по полям
    /// </summary>
    public class GenericFormErrorResult
    {
        /// <summary>
        /// Ошибки по полям
        /// </summary>
        public GenericFormErrors Errors { get; set; }
    }
}
