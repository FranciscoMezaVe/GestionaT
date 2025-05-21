namespace GestionaT.Application.Interfaces.Auth
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? Email { get; }
        List<string> BusinessIds { get; }
    }
}
