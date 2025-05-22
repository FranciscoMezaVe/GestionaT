using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Roles.Commands.CreateRolesCommand
{
    public record CreateRolesCommand : IRequest<Result<Guid>>
    {
        public required string Name { get; set; }
        public Guid BusinessId { get; set; }
    }
}
