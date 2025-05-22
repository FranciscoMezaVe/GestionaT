using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Auth.Commands.RefreshTokenCommand
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<RefreshTokenCommandResponse>>;
}
