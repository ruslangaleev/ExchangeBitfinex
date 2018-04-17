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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeBitfinex.Controllers
{
    [Authorize]
    [Route("api/auth")]
    public class AuthorizationController : Controller
    {
        private static string _refreshToken;

        private readonly AuthOptions _authOptions;

        /// <summary>
        /// Менеджер управления учётками.
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// API для входа пользователя.
        /// </summary>
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthorizationController(IOptions<AuthOptions> options,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _authOptions = options.Value;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary> 
        /// Создает токен для доступа к ресурсам 
        /// </summary> 
        [AllowAnonymous]
        [HttpPost("token")]
        public async Task<object> GetToken([FromBody]LoginModel model)
        {
            //if (!ModelState.IsValid)

            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(nameof(model.Email), "Пользователь с указанным email не найден");
                //throw new ModelErrorException(ModelState);
            }

            var passwordIsValid = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!passwordIsValid)
            {
                ModelState.AddModelError(nameof(model.Password), "Пароль не верный");
                //throw new ModelErrorException(ModelState);
            }

            var isLockedOut = await _userManager.IsLockedOutAsync(user);

            if (!isLockedOut)
            {
                return GenerateToken(user.Id, "user");
            }

            ModelState.AddModelError(string.Empty, "Ваша учетная запись временно заблокирована");
            return null;
            //throw new ModelErrorException(ModelState);
        }

        /// <summary> 
        /// Создает новый токен для доступа к ресурсам 
        /// </summary> 
        // Example: In request header use "Bearer <refreshToken>" 
        [HttpPost("refreshtoken")]
        public object RefreshToken()
        {
            var userFromClaim = User.Claims.FirstOrDefault(t => t.Type == ClaimsIdentity.DefaultNameClaimType)?.Value;
            if (userFromClaim == null)
                return BadRequest("Не найден идентификатор пользователя");

            var user = _userManager.FindByIdAsync(userFromClaim);
            if (user == null)
            {
                return BadRequest("Указанный пользовтаель не существует");
            }

            return GenerateToken(user.Id.ToString(), "user");
        }

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
    }
}
