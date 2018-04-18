using System.Threading.Tasks;

namespace ExchangeBitfinex.Data.Infrastructure
{
    /// <summary> 
    /// Единица работы 
    /// </summary> 
    public interface IUnitOfWork
    {
        /// <summary> 
        /// Сохраняет все изменения 
        /// </summary> 
        Task SaveAsync();
    }
}
