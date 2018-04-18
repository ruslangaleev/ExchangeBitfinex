using ExchangeBitfinex.Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeBitfinex.Services.Services
{
    public interface IBitfinexHandler
    {
        void Start();
    }

    public class BitfinexHandler : IBitfinexHandler
    {
        private readonly int _timeOutInMinutes;

        private readonly IServiceProvider _serviceProvider;

        public BitfinexHandler(IConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            // TODO: tryparse 
            _timeOutInMinutes = int.Parse(configuration["TimeOutInMinutes"] ?? throw new ArgumentNullException("TimeOutInMinutes"));
            _serviceProvider = serviceProvider;
        }
        
        public void Start()
        {
            Task.Run(async () =>
            {
                var timeSpan = TimeSpan.FromMinutes(_timeOutInMinutes);

                while (true)
                {
                    await Worker();

                    await Task.Delay(timeSpan);
                }
            });
        }

        private async Task Worker()
        {
            var list = Enum.GetValues(typeof(CurrencyType)).Cast<CurrencyType>();

            foreach (var entry in list)
            {
                using (var serviceScope = _serviceProvider.GetService<IServiceScopeFactory>().CreateScope())
                {
                    var _bitfinexClient = serviceScope.ServiceProvider.GetRequiredService<IBitfinexClient>();
                    var _currencyInfoManager = serviceScope.ServiceProvider.GetRequiredService<ICurrencyInfoManager>();

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
}
