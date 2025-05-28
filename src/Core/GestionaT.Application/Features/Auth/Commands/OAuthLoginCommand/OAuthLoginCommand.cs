using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Auth.Commands.OAuthLoginCommand
{
    public record OAuthLoginCommand(string AccessToken, string Provider) : IRequest<Result<OAuthLoginCommandResponse>>;
}
