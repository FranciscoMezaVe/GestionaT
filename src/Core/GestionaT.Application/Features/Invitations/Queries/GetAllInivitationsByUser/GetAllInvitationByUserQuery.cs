using FluentResults;
using GestionaT.Application.Common.Pagination;
using GestionaT.Application.Features.Invitations.Queries.GetAllInvitations;
using GestionaT.Domain.Enums;
using MediatR;

namespace GestionaT.Application.Features.Invitations.Queries.GetAllInivitationsByUser
{
    public record GetAllInvitationByUserQuery(GetAllInvitationByUserQueryFilters Filters, PaginationFilters PaginationFilters) :IRequest<Result<PaginatedList<InvitationResponse>>>;
}
