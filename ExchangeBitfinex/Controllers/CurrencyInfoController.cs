using ExchangeBitfinex.Data.Models;
using ExchangeBitfinex.Services.Resources;
using ExchangeBitfinex.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExchangeBitfinex.Controllers
{
    /// <summary>
    /// Контроллер, предоставляющий информацию о валютах
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "user")]
    [Route("api/currency")]
    public class CurrencyInfoController : BaseController
    {
        private readonly ICurrencyInfoManager _currencyInfoManager;

        /// <summary>
        /// Контроллер
        /// </summary>
        public CurrencyInfoController(ICurrencyInfoManager currencyInfoManager)
        {
            _currencyInfoManager = currencyInfoManager ?? throw new ArgumentNullException(nameof(ICurrencyInfoManager));
        }

        /// <summary>
        /// Предоставляет информацию о валютах
        /// </summary>
        /// <param name="currencyType">Тип валюты: EHTUSD, BTCUSD</param>
        /// <param name="pageNumber">Номер страницы</param>
        /// <param name="pageSize">Количество записей на странице</param>
        [HttpGet]
        public async Task<object> GetPageList(CurrencyType currencyType, int pageNumber, int pageSize)
        {
            (int total, IEnumerable<CurrencyInfoResource> list) = await _currencyInfoManager.GetPageList(currencyType, pageNumber, pageSize);
            
            return new
            {
                total,
                list
            };
        }

        /// <summary>
        /// Средний курс валюты за последние 24 часа
        /// </summary>
        [HttpGet("avg")]
        public async Task<object> GetAvgDataLastDay()
        {
            return await _currencyInfoManager.GetAvgDataLastDay();
        }
    }
}
