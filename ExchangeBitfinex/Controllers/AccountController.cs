using ExchangeBitfinex.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ExchangeBitfinex.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        /// <summary>
        /// Менеджер управления учётками.
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// API для входа пользователя.
        /// </summary>
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(UserManager<ApplicationUser>));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(SignInManager<ApplicationUser>));
        }

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
                //return RedirectToLocal(returnUrl);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return BadRequest(ModelState);
            }
        }
    }
}
