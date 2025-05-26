using FluentResults;
using GestionaT.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace GestionaT.Application.Interfaces.Images
{
    /// <summary>
    /// Se encarga de gestionar el almacenamiento de imágenes de productos en la CDN usando la IImageStorageService y en la BD usando IUnitOfWork.
    /// </summary>
    public interface IProductImageStorageService
    {
        Task<Result<Product>> UploadProductImagesAsync(Guid productId, List<IFormFile> images, Guid businessId, Product product);
        Task<Result> UpdateProductImageAsync(Product product, List<IFormFile> images, Guid businessId, Guid entityId);
        Task<Result> DeleteProductImageAsync(Guid productImageId);
    }
}
