using System;
using System.Threading.Tasks;
using Dapper;
using Discount.API.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Discount.API.Repos
{
    public class DiscountRepo : IDiscountRepo
    {
        private readonly string _conStr;

        public DiscountRepo(IConfiguration config)
        {
            config = config ?? throw new ArgumentNullException(nameof(config));
            _conStr = config.GetValue<string>("DatabaseSettings:ConnectionString");
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            using (var con = new NpgsqlConnection(_conStr))
            {
                var coupon = await con.QueryFirstOrDefaultAsync<Coupon>
                ("SELECT * FROM Coupon WHERE ProductName = @ProductName",
                    new {ProductName = productName});
                if (coupon is null)
                {
                    coupon = new Coupon
                    {
                        ProductName = "No Discount",
                        Description = "No desc",
                        Amount = 0
                    };
                }

                return coupon;
            }
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using (var con = new NpgsqlConnection(_conStr))
            {
                var affected = await con.ExecuteAsync(
                    "INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
                    coupon);
                return affected != 0;
            }
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using (var con = new NpgsqlConnection(_conStr))
            {
                var affected = await con.ExecuteAsync(
                    "UPDATE Coupon SET = ProductName = @ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id",
                    coupon);
                return affected != 0;
            }
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using (var con = new NpgsqlConnection(_conStr))
            {
                var affected = await con.ExecuteAsync(
                    "DELETE FROM Coupon WHERE ProductName = @ProductName", new {ProductName = productName});
                return affected != 0;
            }
        }
    }
}