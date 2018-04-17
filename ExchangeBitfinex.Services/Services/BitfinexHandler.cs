using ExchangeBitfinex.Data.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeBitfinex.Services.Services
{
    public class BitfinexHandler
    {
        private readonly int _timeOutInMinutes;

        private readonly IBitfinexClient _bitfinexClient;

        private readonly ICurrencyInfoManager _currencyInfoManager;

        public BitfinexHandler(IConfiguration configuration,
            IBitfinexClient bitfinexClient,
            ICurrencyInfoManager currencyInfoManager)
        {
            // TODO: tryparse 
            _timeOutInMinutes = int.Parse(configuration["TimeOutInMinutes"] ?? throw new ArgumentNullException("TimeOutInMinutes"));
            _bitfinexClient = bitfinexClient ?? throw new ArgumentNullException(nameof(IBitfinexClient));
            _currencyInfoManager = currencyInfoManager ?? throw new ArgumentNullException(nameof(ICurrencyInfoManager));
        }

        // TODO: Должен постоянно работать 
        public async Task Start()
        {
            while (true)
            {
                await Worker();
                Thread.Sleep((int)TimeSpan.FromMinutes(_timeOutInMinutes).TotalMilliseconds);
            }
        }

        private async Task Worker()
        {
            var list = Enum.GetValues(typeof(CurrencyType)).Cast<CurrencyType>();

            foreach (var entry in list)
            {
                var response = await _bitfinexClient.GetInfoCurrency(entry);
                await _currencyInfoManager.AddCurrencyInfo(new CurrencyInfo
                {
                    CurrencyType = entry,
                    DateTime = response.DateTime,
                    LastPrice = response.LastPrice
                });
            }
        }
    }
}
