using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeBitfinex.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("api")]
        public async Task<string> Get()
        {
            return "Сервис предоставления информации о курсах валют от Bitifinex";
        }
    }
}
