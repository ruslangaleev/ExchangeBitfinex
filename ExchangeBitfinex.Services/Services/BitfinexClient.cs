using ExchangeBitfinex.Data.Models;
using ExchangeBitfinex.Services.Resources;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExchangeBitfinex.Services.Services
{
    public interface IBitfinexClient
    {
        Task<CurrencyInfoFromBitfinex> GetInfoCurrency(CurrencyType currencyType);
    }

    public class BitfinexClient : IBitfinexClient
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private readonly Dictionary<CurrencyType, string> _addresses = new Dictionary<CurrencyType, string>
        {
            { CurrencyType.ETHUSD, "/v1/pubticker/ethusd" },
            { CurrencyType.BTCUSD, "/v1/pubticker/btcusd" }
        };

        public BitfinexClient(IConfiguration configuration)
        {
            var baseAddressBitfinex = configuration["BaseUrlBitfinex"] ?? throw new ArgumentNullException("BaseUrlBitfinex");
            _httpClient.BaseAddress = new Uri(baseAddressBitfinex);
        }

        public async Task<CurrencyInfoFromBitfinex> GetInfoCurrency(CurrencyType currencyType)
        {
            try
            {
                var response = await _httpClient.GetAsync(_addresses[currencyType]);
                var json = await response.Content.ReadAsStringAsync();
                var responseBitfinex = JsonConvert.DeserializeObject<ResponseBitfinex>(json);

                var lastPrice = decimal.Parse(responseBitfinex.last_price.Replace('.', ','));
                var timeStamp = double.Parse(responseBitfinex.timestamp.Replace('.', ','));
                var dateTime = UnixTimeStampToDateTime(timeStamp);

                return new CurrencyInfoFromBitfinex
                {
                    // TODO: tryparse 
                    LastPrice = lastPrice,
                    DateTime = dateTime
                };
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
