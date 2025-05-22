using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Members.Commands.CreateMembersCommand
{
    public class CreateMembersCommand : IRequest<Result<Guid>>
    {
        public Guid UserId { get; set; }
        public Guid BusinessId { get; set; }
        public Guid RoleId { get; set; }
    }
}
