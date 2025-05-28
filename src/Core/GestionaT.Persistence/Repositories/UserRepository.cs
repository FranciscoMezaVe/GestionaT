using GestionaT.Application.Common.Models;
using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Persistence.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestionaT.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IQueryable<ApplicationUser> Query()
            => _userManager.Users.AsQueryable();

        private static UserDto ProjectToDto(ApplicationUser user) => new()
        {
            Id = user.Id,
            Email = user.Email!,
            Nombre = user.UserName,
            EmailConfirmado = user.EmailConfirmed
        };

        public async Task<UserDto?> GetByIdAsync(Guid userId)
        {
            var user = await Query().FirstOrDefaultAsync(u => u.Id == userId);
            return user is null ? null : ProjectToDto(user);
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            return await Query().Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email!,
                Nombre = u.UserName,
                EmailConfirmado = u.EmailConfirmed
            }).ToListAsync();
        }

        /// <summary>
        /// Busca usuarios en memoria, debes tener mucho cuidado con muchos datos.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<List<UserDto>> SearchAsync(Func<UserDto, bool> predicate)
        {
            // Traer todos y filtrar en memoria (cuidado con muchos datos)
            var all = await GetAllAsync();
            return all.Where(predicate).ToList();
        }

        public async Task<bool> AnyAsync(Func<UserDto, bool> predicate)
        {
            var all = await GetAllAsync();
            return all.Any(predicate);
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await Query().FirstOrDefaultAsync(u => u.Email == email);
            return user is null ? null : ProjectToDto(user);
        }
    }
}
