using Archetypical.Software.SchemaRegistry.Shared.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Archetypical.Software.SchemaRegistry.Data.Sqlite
{
    public static class StartupHelpers
    {
        public static IServiceCollection AddSqlite(this IServiceCollection services)
        {
            services.AddDbContext<Context>(options =>
            {
                options.UseSqlite("Data Source=schemas.db");
            });

            return services;
        }
    }
}