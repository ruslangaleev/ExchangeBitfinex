using ExchangeBitfinex.Data.Infrastructure;
using ExchangeBitfinex.Data.Models;
using ExchangeBitfinex.Data.Repositories;
using ExchangeBitfinex.Services.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeBitfinex.Services.Services
{
    public interface ICurrencyInfoManager
    {
        Task AddCurrencyInfo(CurrencyInfo currencyInfo);

        Task<(int, IEnumerable<CurrencyInfoResource>)> GetPageList(CurrencyType currencyType, int pageNumber = 1, int pageSize = 50);

        Task<IEnumerable<AvgData>> GetAvgDataLastDay();
    }

    public class CurrencyInfoManager : ICurrencyInfoManager
    {
        private readonly ICurrencyInfoRepository _currencyInfoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CurrencyInfoManager(ICurrencyInfoRepository currencyInfoRepository, IUnitOfWork unitOfWork, IStorageContext storageContext)
        {
            _currencyInfoRepository = currencyInfoRepository ?? throw new ArgumentNullException(nameof(ICurrencyInfoRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(IUnitOfWork));
        }

        public async Task AddCurrencyInfo(CurrencyInfo currencyInfo)
        {
            await _currencyInfoRepository.Add(currencyInfo);

            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<AvgData>> GetAvgDataLastDay()
        {
            var now = DateTime.Now;

            var result = await _currencyInfoRepository.Get(t => t.DateTime <= now && t.DateTime >= (now.AddDays(-1)));

            return result.GroupBy(t => t.CurrencyType).Select(t => new AvgData
            {
                CurrencyType = t.Key.ToString(),
                ArgPrice = t.Average(p => p.LastPrice)
            });
        }

        public async Task<(int, IEnumerable<CurrencyInfoResource>)> GetPageList(CurrencyType currencyType, int pageNumber = 1, int pageSize = 50)
        {
            var total = await _currencyInfoRepository.Count(t => t.CurrencyType == currencyType);
            var list = await _currencyInfoRepository.Get(t => t.CurrencyType == currencyType, pageNumber, pageSize);

            List<CurrencyInfoResource> result = new List<CurrencyInfoResource>();
            foreach(var entry in list)
            {
                result.Add(new CurrencyInfoResource
                {
                    LastPrice = entry.LastPrice,
                    DateTime = entry.DateTime.ToString("dd.MM.yyyy hh:mm:ss")
                });
            }

            return (total, result);
        }
    }
}
