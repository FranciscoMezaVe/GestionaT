using GestionaT.Application.Common.Models;

namespace GestionaT.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<UserDto?> GetByIdAsync(Guid userId);
        Task<List<UserDto>> GetAllAsync();
        Task<List<UserDto>> SearchAsync(Func<UserDto, bool> predicate);
        Task<bool> AnyAsync(Func<UserDto, bool> predicate);
    }
}
