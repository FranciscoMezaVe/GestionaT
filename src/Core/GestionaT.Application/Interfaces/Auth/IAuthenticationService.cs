using FluentResults;
using GestionaT.Domain.ValueObjects;

namespace GestionaT.Application.Interfaces.Auth
{
    public interface IAuthenticationService
    {
        Task<bool> Authenticate(string email, string password);
        Task<Guid> GetUserIdAsync(string email);
        Task<string> GetUserEmailAsync(Guid userId);
        Task<IList<string>> GetUserRolesAsync(Guid userId);
        Task<bool> IsExistsUserByEmailAsync(string email);
        Task<bool> IsExistsUserByIdAsync(Guid id);
        Task<Result<Guid>> RegisterUserAsync(string email, string userName, string password);
        Task<Result<Guid>> RegisterUserOAuthAsync(OAuthUserInfoResult user);
        Task<bool> ConfirmEmailAsync(string email, string token);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
        Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword);
        Task<IList<string>?> GetBusinessesIdAsync(Guid userId);
    }
}
