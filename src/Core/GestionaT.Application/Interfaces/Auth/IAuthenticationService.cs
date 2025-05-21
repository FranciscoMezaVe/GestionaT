namespace GestionaT.Application.Interfaces.Auth
{
    public interface IAuthenticationService
    {
        Task<bool> Authenticate(string email, string password);
        Task<Guid> GetUserIdAsync(string email);
        Task<IList<string>> GetUserRolesAsync(Guid userId);
    }
}
