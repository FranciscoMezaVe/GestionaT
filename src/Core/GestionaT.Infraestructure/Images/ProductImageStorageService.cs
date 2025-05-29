using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Interfaces.Images;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace GestionaT.Infraestructure.Images
{
    public class ProductImageService : IProductImageStorageService
    {
        private readonly IImageStorageService _imageStorageService;
        private readonly IUnitOfWork _unitOfWork;

        public ProductImageService(IImageStorageService imageStorageService, IUnitOfWork unitOfWork)
        {
            _imageStorageService = imageStorageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Product>> UploadProductImagesAsync(Guid productId, List<IFormFile> images, Guid businessId, Product product)
        {
            if (images == null || images.Count == 0)
                return Result.Fail(AppErrorFactory.BadRequest("Debes enviar al menos una imagen."));

            if (product == null)
                return Result.Fail(AppErrorFactory.NotFound(nameof(productId), productId));

            var existingCount = product.Images.Count;
            var totalAfterUpload = existingCount + images.Count;

            if (totalAfterUpload > 5)
                return Result.Fail(AppErrorFactory.BadRequest($"Este producto ya tiene {existingCount} imágenes. Solo puedes subir {5 - existingCount} más."));

            foreach (var image in images)
            {
                var url = await _imageStorageService.SaveImageAsync(image, image.FileName, businessId, nameof(product), productId);
                var publicId = _imageStorageService.ExtractPublicIdFromUrl(url);

                product.Images.Add(new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    ImageUrl = url,
                    PublicId = publicId
                });
            }

            return Result.Ok(product);
        }

        public async Task<Result> UpdateProductImageAsync(Product product, List<IFormFile> images, Guid businessId, Guid entityId)
        {
            foreach(var image in images)
            {
                var newUrl = await _imageStorageService.SaveImageAsync(image, image.FileName, businessId, nameof(product), entityId);
                var newPublicId = _imageStorageService.ExtractPublicIdFromUrl(newUrl);

                var productImage = new ProductImage
                {
                    ProductId = product.Id,
                    ImageUrl = newUrl,
                    PublicId = newPublicId
                };

                await _unitOfWork.Repository<ProductImage>().AddAsync(productImage);
            }

            await _unitOfWork.SaveChangesAsync();
            return Result.Ok();
        }

        public async Task<Result> DeleteProductImageAsync(Guid productImageId)
        {
            var imageRepo = _unitOfWork.Repository<ProductImage>();

            var image = await imageRepo.GetByIdAsync(productImageId);
            if (image == null)
                return Result.Fail(AppErrorFactory.NotFound(nameof(productImageId), productImageId));

            var deleteResult = await _imageStorageService.DeleteImageAsync(image.PublicId);

            if (!deleteResult)
            {
                throw new Exception("No se pudo eliminar la imagen del almacenamiento externo.");
            }
            
            imageRepo.Remove(image);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
