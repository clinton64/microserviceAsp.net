using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Discount.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvailability = retry.Value;
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();
                try
                {
                    logger.LogInformation("Migrating postgresql database");
                    using var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                    connection.Open();
                    using var command = new NpgsqlCommand()
                    {
                        Connection = connection
                    };
                    command.CommandText = "Drop Table If Exists Coupon";
                    command.ExecuteNonQuery();
                    command.CommandText = @"Create Table Coupon(Id Serial Primary Key, ProductName Varchar(24) not null, Description Text, Amount Int)";
                    command.ExecuteNonQuery();
                    command.CommandText = "Insert Into Coupon(ProductName, Description, Amount) Values('IPhone X', 'IPhone Discount', 150);";
                    command.ExecuteNonQuery();
                    command.CommandText = "Insert Into Coupon(ProductName, Description, Amount) Values('Samsung 10', 'Samsung Discount', 100);";
                    command.ExecuteNonQuery();
                    logger.LogInformation("Migrated postgresql database");
                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, "An error occured while migrating the postgresql database");
                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvailability);
                    }
                }
            }
            return host;
        }

    }
}
