using ExchangeBitfinex.Documentation;
using ExchangeBitfinex.Models;
using ExchangeBitfinex.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeBitfinex.Controllers
{
    /// <summary>
    /// Контроллер по управлению авторизацией.
    /// </summary>
    [Authorize]
    [Route("api/auth")]
    public class AuthorizationController : BaseController
    {
        #region Поля

        /// <summary>
        /// Токен обновления.
        /// </summary>
        private static string _refreshToken;

        /// <summary>
        /// Конфигурация для JWT.
        /// </summary>
        private readonly AuthOptions _authOptions;

        /// <summary>
        /// Менеджер управления учётками.
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        #endregion

        #region Конструктор

        /// <summary>
        /// Контроллер.
        /// </summary>
        public AuthorizationController(IOptions<AuthOptions> options,
            UserManager<ApplicationUser> userManager)
        {
            _authOptions = options.Value;
            _userManager = userManager;
        }

        #endregion

        #region Методы (public)

        /// <summary> 
        /// Создает токен для доступа к ресурсам 
        /// </summary> 
        /// <response code="200">Токен успешно сгенерирован.</response>
        /// <response code="400">Ошибка получения токена.</response>
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(GenericFormErrorResult), (int)HttpStatusCode.BadRequest)]
        [HttpPost("token")]
        public async Task<object> GetToken([FromBody]LoginModel model)
        {
            if (ModelState.IsValid)
            {

                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(nameof(model.Email), "Пользователь с указанным email не найден");

                }
                else
                if (!await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    ModelState.AddModelError(nameof(model.Password), "Пароль не верный");
                }
                else
                if (!await _userManager.IsLockedOutAsync(user))
                {
                    return GenerateToken(user.Id, "user");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Ваша учетная запись временно заблокирована");
                }

            }

            return new BadRequestObjectResult(new
            {
                Errors = GetModelState()
            });
        }

        /// <summary> 
        /// Создает новый токен для доступа к ресурсам.
        /// </summary> 
        /// <response code="200">Токен успешно сгенерирован.</response>
        /// <response code="400">Ошибка получения токена.</response>
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(GenericFormErrorResult), (int)HttpStatusCode.BadRequest)]
        [HttpPost("refreshtoken")]
        public object RefreshToken()
        {
            var userFromClaim = User.Claims.FirstOrDefault(t => t.Type == ClaimsIdentity.DefaultNameClaimType)?.Value;
            if (userFromClaim == null)
            {
                ModelState.AddModelError(string.Empty, "Не найден идентификатор пользователя");
            }
            else
            {
                var user = _userManager.FindByIdAsync(userFromClaim);
                if (user != null)
                {
                    return GenerateToken(user.Id.ToString(), "user");
                }

                ModelState.AddModelError(string.Empty, "Указанный пользовтаель не существует");
            }

            return new BadRequestObjectResult(new
            {
                Errors = GetModelState()
            });
        } 

        #endregion

        #region Методы (private)

        /// <summary>
        /// Генерирует токен для доступа к ресурсам и токен обновления.
        /// </summary>
        private AuthorizationTokenResource GenerateToken(string userId, string role)
        {
            var (generatedAccessToken, expires) = GetAccessToken(userId, role);
            var generatedRefreshToken = GetRefreshToken(userId);

            return new AuthorizationTokenResource
            {
                AccessToken = generatedAccessToken,
                RefreshToken = generatedRefreshToken
            };
        }

        /// <summary>
        /// Генерирует токен для доступа к ресурсам.
        /// </summary>
        private (string, TimeSpan) GetAccessToken(string userId, string role)
        {
            Claim[] claims = new Claim[]
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userId),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
            };

            var expires = TimeSpan.FromMinutes(_authOptions.LifeTime);
            var now = DateTime.UtcNow;
            // создаем JWT-токен 
            var accessToken = new JwtSecurityToken(
                    issuer: _authOptions.Issuer,
                    audience: _authOptions.Audience,
                    notBefore: now,
                    claims: claims,
                    expires: now.Add(expires),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_authOptions.Key)),
                        SecurityAlgorithms.HmacSha256));
            return (new JwtSecurityTokenHandler().WriteToken(accessToken), expires);
        }

        /// <summary>
        /// Генерирует токен обновления.
        /// </summary>
        private string GetRefreshToken(string userId)
        {
            Claim[] claims = new Claim[]
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userId)
            };

            var now = DateTime.UtcNow;
            var refreshToken = new JwtSecurityToken(
                    issuer: _authOptions.Issuer,
                    audience: _authOptions.Audience,
                    notBefore: now,
                    claims: claims,
                    expires: now.Add(TimeSpan.FromMinutes(30)),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_authOptions.Key)),
                        SecurityAlgorithms.HmacSha256));
            var encodedRefreshJwt = new JwtSecurityTokenHandler().WriteToken(refreshToken);

            return encodedRefreshJwt;
        } 

        #endregion
    }
}
