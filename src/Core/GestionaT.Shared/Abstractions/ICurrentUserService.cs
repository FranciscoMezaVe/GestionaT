namespace GestionaT.Shared.Abstractions
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? Email { get; }
        List<string> BusinessIds { get; }
        List<string> Roles { get; }
        List<string> GetClaims(string claimType);
    }
}
