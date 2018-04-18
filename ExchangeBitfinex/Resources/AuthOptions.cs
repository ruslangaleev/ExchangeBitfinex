namespace ExchangeBitfinex.Resources
{
    /// <summary> 
    /// Конфиги для JWT 
    /// </summary> 
    public class AuthOptions
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string Key { get; set; }

        public int LifeTime { get; set; }
    }
}
