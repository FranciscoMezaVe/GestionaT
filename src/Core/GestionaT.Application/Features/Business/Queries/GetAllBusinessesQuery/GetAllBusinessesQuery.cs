using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Business.Queries.GetAllBusinessesQuery
{
    public record GetAllBusinessesQuery(Guid UserId) : IRequest<Result<IEnumerable<BusinessReponse>>>;
}
