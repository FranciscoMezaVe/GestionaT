using Microsoft.AspNetCore.Http;

namespace GestionaT.Application.Features.Business.Commands.UploadBusinessImage
{
    public record UploadBusinessImageCommandRequest(IFormFile Image);
}
