using FluentResults;
using GestionaT.Application.Features.Products.Queries;
using MediatR;

namespace GestionaT.Application.Features.Products.Queries.GetAllProducts
{
    public record GetAllProductsQuery(Guid BusinessId) : IRequest<Result<List<ProductResponse>>>;
}
