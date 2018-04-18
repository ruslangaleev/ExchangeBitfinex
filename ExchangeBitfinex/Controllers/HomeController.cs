using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ExchangeBitfinex.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Инициирует контроллер
        /// </summary>
        /// <param name="configuration"></param>
        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Перенаправляет на страницу в документацией
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            var basePath = _configuration.GetValue<string>("BasePath") ?? "";

            return new RedirectResult($"~{basePath}/api-docs");
        }
    }
}
