using GestionaT.Domain.Abstractions;
using GestionaT.Domain.Entities;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace GestionaT.Persistence.PGSQL
{
    public sealed class AppPostgreSqlDbContext : DbContext
    {
        public AppPostgreSqlDbContext(DbContextOptions<AppPostgreSqlDbContext> options) : base(options)
        {
        }

        public DbSet<Business> Businesses => Set<Business>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Sale> Sales => Set<Sale>();
        public DbSet<SaleProduct> SaleProducts=> Set<SaleProduct>();
        public DbSet<User> Users => Set<User>();

        // Auditado automático
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<IAuditable>();

            foreach (var entry in entries)
            {
                var now = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplica todas las configuraciones de IEntityTypeConfiguration<> automáticamente
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Configuración global para usar GUIDs como claves primarias si lo deseas
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var idProp = entityType.FindProperty("Id");
                if (idProp != null && idProp.ClrType == typeof(Guid))
                {
                    idProp.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
                }
            }
        }
    }
}
