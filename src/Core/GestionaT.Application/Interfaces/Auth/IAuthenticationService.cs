namespace GestionaT.Application.Interfaces.Auth
{
    public interface IAuthenticationService
    {
        Task<bool> Authenticate(string email, string password);
        Task<Guid> GetUserIdAsync(string email);
        Task<IList<string>> GetUserRolesAsync(Guid userId);
        Task<bool> RegisterUserAsync(string email, string password);
        Task<bool> ConfirmEmailAsync(string email, string token);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
        Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword);
        Task<IList<string>?> GetBusinessesIdAsync(Guid userId);
    }
}
