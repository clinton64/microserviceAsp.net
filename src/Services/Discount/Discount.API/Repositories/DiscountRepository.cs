using Dapper;
using Discount.API.Entities;
using Npgsql;

namespace Discount.API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;
        public DiscountRepository(IConfiguration configuration) 
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings.ConnectionString"));
            var affected = await connection.ExecuteAsync("Insert Into Coupon (ProductName, Description, Amount) Values (@ProductName, @Description, @Amount)", new {ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount});
            if (affected == 0)
                return false;
            return true;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings.ConnectionString"));
            var affected = await connection.ExecuteAsync("Delete From Coupon Where ProductName == @ProductName", new { ProductName = productName });
            if (affected == 0)
                return false;
            return true;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(
                _configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon> ("Select * from Coupon Where ProductName = @Productname", new {ProductName = productName});
            if (coupon == null)
                return new Coupon
                {
                    ProductName = "No Discount",
                    Amount = 0,
                    Description = "No Discount desc"
                };
            return coupon;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var affected = await connection.ExecuteAsync("Update Coupon Set ProductName=@ProductName, Description=@Description, Amount=@Amount", new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });
            if (affected == 0)
                return false;
            return true;
        }
    }
}
