using System;

namespace ExchangeBitfinex.Data.Models
{
    /// <summary> 
    /// Информация о валюте 
    /// </summary> 
    public class CurrencyInfo
    {
        /// <summary> 
        /// Тип валюты 
        /// </summary> 
        public CurrencyType CurrencyType { get; set; }

        /// <summary> 
        /// Последняя цена валюты 
        /// </summary> 
        public decimal LastPrice { get; set; }

        /// <summary> 
        /// Время последнего изменения валюты 
        /// </summary> 
        public DateTime DateTime { get; set; }
    }
}
