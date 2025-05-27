using Microsoft.AspNetCore.Http;

namespace GestionaT.Application.Features.Categories.Commands.UploadCategoryImage
{
    public record UploadCategoryImageCommandRequest(IFormFile Image);
}
