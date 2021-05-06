using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Discount.API.Extensions
{
    public static class HostExtensions
    {
        /// <summary>
        /// Migrates database 
        /// </summary>
        /// <param name="host">IHost</param>
        /// <param name="retry">Number of attempts to try migrate, if db's container is not started yet</param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public static IHost MigrateDb<TContext>(this IHost host, int? retry = 0)
        {
            var retryForAvailability = retry ?? 0;
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating PostgreSQL database...");

                    using (var con =
                        new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
                    {
                        con.Open();

                        using (var command = new NpgsqlCommand {Connection = con})
                        {
                            command.CommandText = "DROP TABLE IF EXISTS Coupon";
                            command.ExecuteNonQuery();

                            command.CommandText = 
                                @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY,
                                    ProductName VARCHAR(24) NOT NULL,
                                    Description TEXT,
                                    Amount INT
                                )";
                            command.ExecuteNonQuery();

                            command.CommandText =
                                "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('IPhone X', 'IPhone Discount', 150);";
                            command.ExecuteNonQuery();
                            
                            command.CommandText =
                                "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Sumsung 10', 'Sumsung Discount', 100);";
                            command.ExecuteNonQuery();
                        }
                    }
                    
                    logger.LogInformation("Migrated postgreSQL successfully");
                }
                catch (NpgsqlException e)
                {
                    logger.LogError(e, "An error occured while migrating db");
                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        Thread.Sleep(2000);
                        MigrateDb<TContext>(host, retryForAvailability);
                    }
                }
            }

            return host;
        }
    }
}