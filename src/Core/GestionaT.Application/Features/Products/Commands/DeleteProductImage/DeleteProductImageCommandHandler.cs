using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Interfaces.Images;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Products.Commands.DeleteProductImage
{
    public class DeleteProductImageCommandHandler : IRequestHandler<DeleteProductImageCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductImageStorageService _productImageService;
        private readonly ILogger<DeleteProductImageCommandHandler> _logger;

        public DeleteProductImageCommandHandler(IUnitOfWork unitOfWork,
            IProductImageStorageService productImageService,
            ILogger<DeleteProductImageCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _productImageService = productImageService;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteProductImageCommand command, CancellationToken cancellationToken)
        {
            var productRepo = _unitOfWork.Repository<Product>();
            var imageRepo = _unitOfWork.Repository<ProductImage>();

            // Validar que el producto exista y pertenezca al negocio
            var product = productRepo.Query()
                .FirstOrDefault(p => p.Id == command.ProductId && p.BusinessId == command.BusinessId);

            if (product == null)
            {
                _logger.LogWarning("Producto no encontrado o no pertenece al negocio. ProductId: {ProductId}, BusinessId: {BusinessId}", command.ProductId, command.BusinessId);
                return Result.Fail(new HttpError("Producto no encontrado.", ResultStatusCode.NotFound));
            }

            // Validar que la imagen exista y pertenezca al producto
            var image = imageRepo.Query()
                .FirstOrDefault(img => img.Id == command.ProductImageId && img.ProductId == command.ProductId);

            if (image == null)
            {
                _logger.LogWarning("Imagen no encontrada o no pertenece al producto. ImageId: {ImageId}, ProductId: {ProductId}", command.ProductImageId, command.ProductId);
                return Result.Fail(new HttpError("Imagen no encontrada para el producto especificado.", ResultStatusCode.NotFound));
            }

            // Eliminar la imagen usando el servicio
            var result = await _productImageService.DeleteProductImageAsync(command.ProductImageId);

            if (result.IsFailed)
            {
                _logger.LogError("Error eliminando imagen: {Errors}", string.Join(", ", result.Errors));
                return result;
            }

            _logger.LogInformation("Imagen eliminada correctamente. ImageId: {ImageId}", command.ProductImageId);
            return Result.Ok();
        }
    }
}