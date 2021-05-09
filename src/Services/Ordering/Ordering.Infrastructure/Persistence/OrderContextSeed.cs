using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext context, ILogger<OrderContextSeed> logger)
        {
            if (!context.Orders.Any())
            {
                context.Orders.AddRange(GetOrders());
            }
        }

        private static IEnumerable<Order> GetOrders()
        {
            return Enumerable.Range(1, 1)
                .Select(x => new Order
                {
                    UserName = $"user-{x}",
                    FirstName = $"fname-{x}",
                    LastName = $"lname-{x}",
                    EmailAddress = $"email-{x}",
                    TotalPrice = x * 100
                });
        }
    }
}