using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Products.Commands.DeleteProductImage
{
    public record DeleteProductImageCommand(Guid BusinessId, Guid ProductId, Guid ProductImageId) : IRequest<Result>;
}