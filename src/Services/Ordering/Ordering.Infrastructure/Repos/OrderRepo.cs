using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Repos
{
    public class OrderRepo : RepoBase<Order>, IOrderRepo
    {
        public OrderRepo(OrderContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserName(string userName)
        {
            return await Context.Orders
                .Where(x => x.UserName == userName)
                .ToListAsync();
        }
    }
}