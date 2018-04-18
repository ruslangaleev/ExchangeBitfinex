using Microsoft.AspNetCore.Mvc;

namespace ExchangeBitfinex.Controllers
{
    /// <summary>
    /// Контроллер для перенаправления на страницу с документацией
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Перенаправляет на страницу в документацией
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            return new RedirectResult($"~/api-docs");
        }
    }
}
