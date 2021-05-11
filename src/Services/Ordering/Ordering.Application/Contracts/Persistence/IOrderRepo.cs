using System.Collections.Generic;
using System.Threading.Tasks;
using Ordering.Domain.Entities;

namespace Ordering.Application.Contracts.Persistence
{
    public interface IOrderRepo  : IAsyncRepo<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserName(string userName);
    }
}