using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ExchangeBitfinex.Controllers
{
    /// <summary>
    /// Базовый контроллер
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// Формирует список ошибок модели
        /// </summary>
        /// <returns></returns>
        protected object GetModelState()
        {
            return ModelState.ToDictionary(x => string.IsNullOrEmpty(x.Key.ToCamelCase()) ? "_global" : x.Key, x => x.Value.Errors.Select(y => y.ErrorMessage));
        }
    }

    /// <summary>
    /// Расширения 
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// ToCamelCase
        /// </summary>
        public static string ToCamelCase(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            if (!char.IsUpper(s[0]))
                return s;

            var camelCase = char.ToLower(s[0]).ToString();
            if (s.Length > 1)
                camelCase += s.Substring(1);

            return camelCase;
        }
    }
}
