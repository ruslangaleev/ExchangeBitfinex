namespace ExchangeBitfinex.Services.Resources
{
    /// <summary>
    /// Модель для отображения среднего значения валюты за указанный период
    /// </summary>
    public class AvgData
    {
        /// <summary> 
        /// Тип валюты 
        /// </summary> 
        public string CurrencyType { get; set; }

        /// <summary> 
        /// Средняя цена 
        /// </summary> 
        public decimal ArgPrice { get; set; }
    }
}
