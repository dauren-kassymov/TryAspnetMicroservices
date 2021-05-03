using System.Threading.Tasks;
using Basket.API.Entities;

namespace Basket.API.Repos
{
    public interface IBasketRepo
    {
        Task<ShoppingCart> GetBasket(string userName);
        Task<ShoppingCart> UpdateBasket(ShoppingCart basket);
        Task DeleteBasket(string userName);
    }
}