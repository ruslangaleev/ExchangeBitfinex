namespace ExchangeBitfinex.Services.Resources
{
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
