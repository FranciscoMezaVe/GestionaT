namespace GestionaT.Application.Interfaces.Auth
{
    public interface IJwtTokenService
    {
        string GenerateToken(Guid userId, string userEmail, IList<string> roles);
    }
}
