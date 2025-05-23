using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Members.Queries.GetAllMembersByBusiness
{
    public record GetAllMembersByBusinessQuery(Guid BusinessId) : IRequest<Result<IEnumerable<MembersResponse>>>;
}