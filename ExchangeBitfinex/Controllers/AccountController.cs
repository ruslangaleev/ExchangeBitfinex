using ExchangeBitfinex.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ExchangeBitfinex.Controllers
{
    /// <summary>
    /// Контроллер по управлению пользователями
    /// </summary>
    [Route("account")]
    public class AccountController : Controller
    {
        /// <summary>
        /// Менеджер управления учётками
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Конструктор
        /// </summary>
        public AccountController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(UserManager<ApplicationUser>));
        }

        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        [HttpPost("register")]
        public async Task<object> RegisterAccount([FromBody]RegisterModel model)
        {
            //if (!ModelState.IsValid)
            //    throw new ModelErrorException(ModelState);

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok("Пользователь успешно зарегистрирован");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return BadRequest(ModelState);
            }
        }
    }
}
