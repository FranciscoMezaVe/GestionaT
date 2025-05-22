namespace GestionaT.Application.Features.Auth.Commands.RefreshTokenCommand
{
    public record RefreshTokenCommandResponse(string NewToken, string NewRefreshToken);
}