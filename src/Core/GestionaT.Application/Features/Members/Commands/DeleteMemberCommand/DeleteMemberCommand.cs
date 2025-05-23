using FluentResults;
using GestionaT.Domain.Entities;
using MediatR;

namespace GestionaT.Application.Features.Members.Commands.DeleteMemberCommand
{
    public record DeleteMemberCommand(Guid MemberId, Guid BusinessId) : IRequest<Result>;
}