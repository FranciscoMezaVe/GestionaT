using GestionaT.Domain.Abstractions;
using GestionaT.Domain.Entities;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using GestionaT.Persistence.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using GestionaT.Shared.Abstractions;

namespace GestionaT.Persistence.PGSQL
{
    public sealed class AppPostgreSqlDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        private readonly ICurrentUserService _currentUserService;
        public AppPostgreSqlDbContext(DbContextOptions<AppPostgreSqlDbContext> options, ICurrentUserService currentUserService)
            : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Business> Businesses => Set<Business>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Sale> Sales => Set<Sale>();
        public DbSet<SaleProduct> SaleProducts=> Set<SaleProduct>();
        public DbSet<Members> Members => Set<Members>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();
        public DbSet<CategoryImage> CategoryImages => Set<CategoryImage>();
        public DbSet<BusinessImage> BusinessImages => Set<BusinessImage>();
        public DbSet<UserImage> UserImages => Set<UserImage>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<OAuthProviders> OAuthProviders => Set<OAuthProviders>();

        // Auditado automático
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<IAuditable>();
            var userId = _currentUserService.UserId?.ToString() ?? "system";

            foreach (var entry in entries)
            {
                var now = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = userId;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = userId;
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
