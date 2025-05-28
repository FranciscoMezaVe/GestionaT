using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using GestionaT.Shared.Abstractions;

namespace GestionaT.Persistence.PGSQL
{
    public class DesignTimeDbContextFactory
    {
        public AppPostgreSqlDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppPostgreSqlDbContext>();
            optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("PGSQLConnectionString"));

            var fakeCurrentUserService = new FakeCurrentUserService();

            return new AppPostgreSqlDbContext(optionsBuilder.Options, fakeCurrentUserService);
        }

        public class FakeCurrentUserService : ICurrentUserService
        {
            public Guid? UserId => Guid.Empty;

            public string? Email => "design-time@example.com";

            public List<string> BusinessIds => new List<string>();

            public List<string> Roles => new List<string> { "Admin" };

            public List<string> GetClaims(string claimType)
            {
                return new List<string>(); // Retorna lista vacía o simula algunos claims si es necesario
            }
        }
    }
}
