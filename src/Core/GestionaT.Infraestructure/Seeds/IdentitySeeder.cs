using GestionaT.Persistence.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestionaT.Infraestructure.Seeds
{
    public static class IdentitySeeder
    {
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            const string sectionName = "SeedAdminUser";
            const string emailName = "Email";
            const string passwordName = "Password";
            const string roleName = "Role";
            const string userName = "UserName";

            using var scope = serviceProvider.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var seedAdminUser = config.GetSection(sectionName);

            // Verifica que la sección exista y tenga datos
            if (!seedAdminUser.Exists() ||
                string.IsNullOrWhiteSpace(seedAdminUser[emailName]) ||
                string.IsNullOrWhiteSpace(seedAdminUser[passwordName]) ||
                string.IsNullOrWhiteSpace(seedAdminUser[roleName]) ||
                string.IsNullOrWhiteSpace(seedAdminUser[userName]))
            {
                // No hay configuración válida: no hacer nada
                return;
            }

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            string adminEmail = seedAdminUser[emailName]!;
            string adminPassword = seedAdminUser[passwordName]!;
            string role = seedAdminUser[roleName]!;
            string username = seedAdminUser[userName]!;

            // Crear rol si no existe
            var roleExists = await roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                var newRole = new ApplicationRole
                {
                    Name = role,
                    NormalizedName = role.ToUpperInvariant()
                };
                var roleResult = await roleManager.CreateAsync(newRole);

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    throw new Exception($"Error creating role '{role}': {errors}");
                }
            }

            // Crear usuario si no existe
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = username,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, role);
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Error creating admin user: {errors}");
                }
            }
            else
            {
                // Asegurarse de que tenga el rol
                var roles = await userManager.GetRolesAsync(adminUser);
                if (!roles.Contains(role))
                {
                    await userManager.AddToRoleAsync(adminUser, role);
                }
            }
        }
    }
}
