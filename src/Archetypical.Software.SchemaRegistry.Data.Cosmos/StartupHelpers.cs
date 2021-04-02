using Archetypical.Software.SchemaRegistry.Shared.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Archetypical.Software.SchemaRegistry.Data.Cosmos
{
    public static class StartupHelpers
    {
        public static IServiceCollection AddCosmos(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<Context>(options =>
            {
                options.UseCosmos(
                    accountEndpoint: configuration["Cosmos:AccountEndpoint"],
                    accountKey: configuration["Cosmos:AccountKey"],
                    databaseName: configuration["Cosmos:DatabaseName"],
                    opts =>
                    {
                    });
            });

            return services;
        }
    }
}