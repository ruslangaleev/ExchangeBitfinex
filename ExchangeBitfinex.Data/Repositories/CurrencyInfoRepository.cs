using ExchangeBitfinex.Data.Infrastructure;
using ExchangeBitfinex.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ExchangeBitfinex.Data.Repositories
{
    /// <summary> 
    /// Управление данными валюты 
    /// </summary> 
    public interface ICurrencyInfoRepository
    {
        /// <summary> 
        /// Добавляет новую информацию о валюте 
        /// </summary> 
        Task Add(CurrencyInfo currencyInfo);

        /// <summary> 
        /// Возвращает список информаций о валюте по условию 
        /// </summary> 
        Task<IEnumerable<CurrencyInfo>> Get(Expression<Func<CurrencyInfo, bool>> where = null);

        // TODO: добавить order by
        Task<IEnumerable<CurrencyInfo>> Get(Expression<Func<CurrencyInfo, bool>> where = null, int pageNumber = 1, int pageSize = 50);

        Task<int> Count(Expression<Func<CurrencyInfo, bool>> where = null);
    }

    /// <summary> 
    /// Управление данными валюты 
    /// </summary> 
    public class CurrencyInfoRepository : ICurrencyInfoRepository
    {
        /// <summary> 
        /// Коллекция информаций о валюте 
        /// </summary> 
        private readonly DbSet<CurrencyInfo> _currencyInfos;

        /// <summary> 
        /// Конструктор 
        /// </summary> 
        public CurrencyInfoRepository(IStorageContext storageContext)
        {
            var context = storageContext as ApplicationDbContext;
            _currencyInfos = context.Set<CurrencyInfo>();
        }

        /// <summary> 
        /// Добавляет новую информацию о валюте 
        /// </summary> 
        public async Task Add(CurrencyInfo currencyInfo)
        {
            await _currencyInfos.AddAsync(currencyInfo);
        }

        /// <summary> 
        /// Возвращает список информаций о валюте по условию 
        /// </summary> 
        public async Task<IEnumerable<CurrencyInfo>> Get(Expression<Func<CurrencyInfo, bool>> where = null)
        {
            return await _currencyInfos.Where(where).ToListAsync();
        }

        public async Task<int> Count(Expression<Func<CurrencyInfo, bool>> where = null)
        {
            return await _currencyInfos.CountAsync(where);
        }

        public async Task<IEnumerable<CurrencyInfo>> Get(Expression<Func<CurrencyInfo, bool>> where = null, int pageNumber = 1, int pageSize = 50)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 50;
            }

            return await _currencyInfos
                .Where(where)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(t => t.DateTime) // TODO: добавить order by
                .ToListAsync();
        }
    }
}
