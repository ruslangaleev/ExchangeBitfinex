namespace ExchangeBitfinex.Services.Resources
{
    public class CurrencyInfoResource
    {
        /// <summary> 
        /// Последняя цена валюты 
        /// </summary> 
        public decimal LastPrice { get; set; }

        /// <summary> 
        /// Время последнего изменения валюты 
        /// </summary> 
        public string DateTime { get; set; }
    }
}
