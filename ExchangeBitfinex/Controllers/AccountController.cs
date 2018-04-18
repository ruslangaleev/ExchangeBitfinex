using ExchangeBitfinex.Documentation;
using ExchangeBitfinex.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ExchangeBitfinex.Controllers
{
    /// <summary>
    /// Контроллер по управлению пользователями.
    /// </summary>
    [Route("account")]
    public class AccountController : BaseController
    {
        /// <summary>
        /// Менеджер управления учётками.
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Конструктор.
        /// </summary>
        public AccountController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(UserManager<ApplicationUser>));
        }

        /// <summary>
        /// Регистрация пользователя.
        /// </summary>
        /// <response code="200">Пользователь успешно зарегистрирован.</response>
        /// <response code="400">Ошибка регистрации пользователя.</response>
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(GenericFormErrorResult), (int)HttpStatusCode.BadRequest)]
        [HttpPost("register")]
        public async Task<object> RegisterAccount([FromBody]RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return Ok($"Пользователь {model.Email} успешно зарегистрирован");
                }
                if (result.Errors.FirstOrDefault()?.Code == "DuplicateUserName")
                {
                    ModelState.AddModelError(nameof(model.Email), "Пользователь с указанным email уже зарегистрирован");
                }
            }

            return new BadRequestObjectResult(new
            {
                Errors = GetModelState()
            });
        }
    }
}
