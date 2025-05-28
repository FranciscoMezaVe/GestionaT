using GestionaT.Domain.ValueObjects;

namespace GestionaT.Application.Interfaces.Auth
{
    public interface IOAuthService
    {
        string Provider { get; }
        Task<OAuthUserInfoResult> GetUserInfoAsync(string accessToken);
    }
}
