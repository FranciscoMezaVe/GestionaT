using FluentResults;
using GestionaT.Application.Features.Products.Queries;
using MediatR;

namespace GestionaT.Application.Features.Products.Queries.GetProductById
{
    public record GetProductByIdQuery(Guid BusinessId, Guid Id) : IRequest<Result<ProductResponse>>;
}