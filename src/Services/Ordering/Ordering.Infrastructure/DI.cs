using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Infrastructure.Email;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Repos;

namespace Ordering.Infrastructure
{
    public static class DI
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<OrderContext>(opt =>
                opt.UseSqlServer(configuration.GetConnectionString("OrderingConnectionString")));
            
            services.AddScoped(typeof(IAsyncRepo<>), typeof(RepoBase<>));
            services.AddScoped<IOrderRepo, OrderRepo>();

            services.Configure<EmailSettings>(x => configuration.GetSection("EmailSettings"));//TODO
            services.AddTransient<IEmailService, EmailService>();
            
            return services;
        }
    }
}