using ExchangeBitfinex.Data.Models;
using ExchangeBitfinex.Services.Resources;
using ExchangeBitfinex.Services.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExchangeBitfinex.Controllers
{
    [Route("api/currency")]
    public class CurrencyInfoController : Controller
    {
        private readonly ICurrencyInfoManager _currencyInfoManager;

        public CurrencyInfoController(ICurrencyInfoManager currencyInfoManager)
        {
            _currencyInfoManager = currencyInfoManager ?? throw new ArgumentNullException(nameof(ICurrencyInfoManager));
        }

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

        [HttpGet("avg")]
        public async Task<object> GetAvgDataLastDay()
        {
            return await _currencyInfoManager.GetAvgDataLastDay();
        }
    }
}
