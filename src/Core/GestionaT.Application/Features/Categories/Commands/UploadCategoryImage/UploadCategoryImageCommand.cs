using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace GestionaT.Application.Features.Categories.Commands.UploadCategoryImage
{
    public record UploadCategoryImageCommand(IFormFile Image, Guid BusinessId, Guid CategoryId) : IRequest<Result<string>>;
}
