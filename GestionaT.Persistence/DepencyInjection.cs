using GestionaT.Persistence.PGSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestionaT.Persistence
{
    public static class DepencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            #region PGSQL
            var connectionString = Environment.GetEnvironmentVariable("PGSQLConnectionString");
            services.AddDbContext<AppPostgreSqlDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
            #endregion

            return services;
        }
    }
}
