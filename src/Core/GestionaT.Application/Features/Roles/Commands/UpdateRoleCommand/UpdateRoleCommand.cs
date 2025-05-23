using FluentResults;
using GestionaT.Domain.Entities;
using MediatR;

namespace GestionaT.Application.Features.Roles.Commands.UpdateRoleCommand
{
    public class UpdateRoleCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string Name { get; set; } = default!;
    }
}