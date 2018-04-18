﻿using System.Threading.Tasks;

namespace ExchangeBitfinex.Data.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary> 
        /// Контекст данных 
        /// </summary> 
        private ApplicationDbContext _applicationDbContext;

        /// <summary> 
        /// Конструктор 
        /// </summary> 
        public UnitOfWork(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            //_applicationDbContext = storageContext as ApplicationDbContext;
        }

        /// <summary> 
        /// Сохраняет все изменения 
        /// </summary> 
        public async Task SaveAsync()
        {
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
