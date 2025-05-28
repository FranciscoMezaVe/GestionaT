using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Auth.Commands.LogoutCommand
{
    public record LogoutCommand() : IRequest<Result>;
}
