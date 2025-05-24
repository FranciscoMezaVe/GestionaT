using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Products.Commands.UpdateProduct
{
    public record UpdateProductCommand(UpdateProductCommandRequest Request, Guid ProductId, Guid BusinessId) : IRequest<Result>;
}
