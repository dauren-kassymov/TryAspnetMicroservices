using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Ordering.API.Extensions;
using Ordering.Infrastructure.Persistence;

namespace Ordering.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .MigrateDb<OrderContext>(Seed)
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void Seed(OrderContext context, IServiceProvider services)
        {
            var logger = services.GetService<ILogger<OrderContextSeed>>();
            OrderContextSeed.SeedAsync(context, logger).Wait();
        }
    }
}
