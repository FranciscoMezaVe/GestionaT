using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using GestionaT.Infraestructure.Common.Images;
using GestionaT.Application.Interfaces.Images;
using Microsoft.AspNetCore.Http;

namespace GestionaT.Infraestructure.Images
{
    public class CloudinaryImageStorageService : IImageStorageService
    {
        private readonly Cloudinary _cloudinary;
        private const string rootName = "GestionaT";

        public CloudinaryImageStorageService(IOptions<CloudinarySettings> config)
        {
            var cloudName = Environment.GetEnvironmentVariable(config.Value.CloudName);
            var apiKey = Environment.GetEnvironmentVariable(config.Value.ApiKey);
            var apiSecret = Environment.GetEnvironmentVariable(config.Value.ApiSecret);

            if (string.IsNullOrWhiteSpace(cloudName) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
                throw new Exception("Faltan variables de entorno de configuración de Cloudinary.");

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account) { Api = { Secure = true } };
        }

        public async Task<string> SaveImageAsync(IFormFile image, string fileName, Guid businessId, string entity, Guid entityId)
        {
            var folder = $"{rootName}/{businessId}/{entity}/{entityId}"; // Ej: 03a2d.../products/03a2d... o users/f4e3...
            using var stream = image.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false,
                Folder = folder
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                return result.SecureUrl.ToString();

            throw new Exception("No se pudo guardar la imagen en Cloudinary.");
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParams);
            return result.Result == "ok" || result.Result == "not_found";
        }

        public string ExtractPublicIdFromUrl(string url)
        {
            var uri = new Uri(url);
            var parts = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            var productIndex = Array.IndexOf(parts, rootName);
            if (productIndex == -1 || productIndex + 1 >= parts.Length)
                return string.Empty;

            var relevantParts = parts.Skip(productIndex);
            var publicIdWithFile = string.Join("/", relevantParts);

            // Quitar extensión (.png, .jpg, .jpeg, etc.)
            return System.IO.Path.ChangeExtension(publicIdWithFile, null);
        }
    }
}
