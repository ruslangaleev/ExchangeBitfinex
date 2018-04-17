using System;

namespace ExchangeBitfinex.Services.Resources
{
    public class CurrencyInfoFromBitfinex
    {
        public decimal LastPrice { get; set; }

        public DateTime DateTime { get; set; }
    }
}
