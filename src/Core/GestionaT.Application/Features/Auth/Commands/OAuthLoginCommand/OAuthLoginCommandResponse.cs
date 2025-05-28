namespace GestionaT.Application.Features.Auth.Commands.OAuthLoginCommand
{
    public record OAuthLoginCommandResponse(string Token, string RefreshToken);
}