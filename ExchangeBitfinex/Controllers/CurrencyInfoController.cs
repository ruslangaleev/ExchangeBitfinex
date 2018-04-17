using ExchangeBitfinex.Data.Models;
using ExchangeBitfinex.Services.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExchangeBitfinex.Controllers
{
    [Route("api")]
    public class CurrencyInfoController : Controller
    {
        private readonly ICurrencyInfoManager _currencyInfoManager;

        public CurrencyInfoController(ICurrencyInfoManager currencyInfoManager)
        {
            _currencyInfoManager = currencyInfoManager ?? throw new ArgumentNullException(nameof(ICurrencyInfoManager));
        }

        [HttpPost("currency")]
        public async Task<object> GetPageList(CurrencyType currencyType, int pageNumber, int pageSize)
        {
            (int count, IEnumerable<CurrencyInfo> list) = await _currencyInfoManager.GetPageList(currencyType, pageNumber, pageSize);

            return new
            {
                count,
                list
            };
        }
    }
}
