using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Products.Commands.CreateProduct
{
    public record CreateProductCommand(CreateProductCommandRequest Request, Guid BusinessId) : IRequest<Result<Guid>>;
}
