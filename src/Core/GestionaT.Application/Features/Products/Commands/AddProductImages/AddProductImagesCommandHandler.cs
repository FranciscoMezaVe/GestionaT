using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Interfaces.Images;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Products.Commands.AddProductImages
{
    public class AddProductImagesCommandHandler : IRequestHandler<AddProductImagesCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductImageStorageService _productImageService;
        private readonly ILogger<AddProductImagesCommandHandler> _logger;

        public AddProductImagesCommandHandler(
            IUnitOfWork unitOfWork,
            IProductImageStorageService productImageService,
            ILogger<AddProductImagesCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _productImageService = productImageService;
            _logger = logger;
        }

        public async Task<Result> Handle(AddProductImagesCommand command, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();
            var productRepo = _unitOfWork.Repository<Product>();

            var product = productRepo.Include(p => p.Images)
                .FirstOrDefault(p => p.Id == command.ProductId && p.BusinessId == command.BusinessId);

            if (product is null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Fail(AppErrorFactory.NotFound(nameof(command.ProductId), command.ProductId));
            }

            var currentImagesCount = product.Images?.Count ?? 0;
            if (currentImagesCount + command.Images.Count > 5)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Fail("No puedes tener más de 5 imágenes en total por producto.");
            }

            var result = await _productImageService.UpdateProductImageAsync(product, command.Images, command.BusinessId, product.Id);

            if (result.IsFailed)
            {
                var errorMessages = string.Join(" | ", result.Errors.Select(e => e.Message));
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Fail(AppErrorFactory.Internal(errorMessages ?? "No se pudo subir la imagen"));
            }

            await _unitOfWork.CommitTransactionAsync();
            return Result.Ok();
        }
    }
}
