using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Auth.Commands.RegisterCommand
{
    public record RegisterCommand(RegisterCommandRequest request) : IRequest<Result<Guid>>;
}
