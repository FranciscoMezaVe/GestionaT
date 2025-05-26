using FluentResults;
using GestionaT.Application.Common.Pagination;
using MediatR;

namespace GestionaT.Application.Features.Invitations.Queries.GetAllInvitations
{
    public record GetAllInvitationsQuery(Guid businessId, PaginationFilters Filters) : IRequest<Result<PaginatedList<InvitationResponse>>>;
}
