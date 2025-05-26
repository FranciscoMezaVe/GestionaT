using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace GestionaT.Application.Features.Products.Commands.AddProductImages
{
    public record AddProductImagesCommand(
        Guid BusinessId,
        Guid ProductId,
        List<IFormFile> Images
    ) : IRequest<Result>;
}