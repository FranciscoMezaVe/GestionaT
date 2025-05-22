using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Business.Queries.GetBusinessByIdQuery
{
    public record GetBusinessByIdQuery(Guid BusinessId) : IRequest<Result<BusinessReponse>>;
}
