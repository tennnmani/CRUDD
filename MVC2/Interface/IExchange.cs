using MVC2.Models;

namespace MVC2.Interface
{
    public interface IExchange
    {
        Task<List<Exchange>> callExchange();

        Task<Decimal> getRate(string fromRate , string toRate , string buySell);
    }
}
