using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Members.Queries.GetMemberById
{
    public record GetMemberByIdQuery(Guid BusinessId, Guid MemberId) : IRequest<Result<MembersResponse>>;
}