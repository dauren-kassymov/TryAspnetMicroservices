using System;
using System.Threading;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ordering.API.Extensions
{
    public static class HostExtensions
    {
        /// <summary>
        /// Migrates database 
        /// </summary>
        /// <param name="host">IHost</param>
        /// <param name="seeder">Action to seed db</param>
        /// <param name="retry">Number of attempts to try migrate, if db's container is not started yet</param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public static IHost MigrateDb<TContext>(this IHost host,
            Action<TContext, IServiceProvider> seeder,
            int? retry = 0) where TContext : DbContext
        {
            var retryForAvailability = retry ?? 0;
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation("Migrating SQLServer database...");

                    Migrate(context, seeder, services);

                    logger.LogInformation("Migrated SQLServer successfully");
                }
                catch (SqlException e)
                {
                    logger.LogError(e, "An error occurred while migrating db");
                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        Thread.Sleep(2000);
                        MigrateDb(host, seeder, retryForAvailability);
                    }
                }
            }

            return host;
        }

        private static void Migrate<TContext>(
            TContext context,
            Action<TContext, IServiceProvider> seeder,
            IServiceProvider services) where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, services);
        }
    }
}