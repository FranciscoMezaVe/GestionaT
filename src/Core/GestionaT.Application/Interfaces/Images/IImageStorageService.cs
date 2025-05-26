using Microsoft.AspNetCore.Http;

namespace GestionaT.Application.Interfaces.Images
{
    public interface IImageStorageService
    {
        Task<string> SaveImageAsync(IFormFile image, string fileName, Guid businessId, string entity, Guid entityId);
        Task<bool> DeleteImageAsync(string publicId);
        string ExtractPublicIdFromUrl(string url);
    }
}
