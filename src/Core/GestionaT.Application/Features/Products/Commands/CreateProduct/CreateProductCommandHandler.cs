using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Interfaces.Images;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateProductCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IProductImageStorageService _productImageStorageService;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateProductCommandHandler> logger, IMapper mapper, IProductImageStorageService productImageStorageService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _productImageStorageService = productImageStorageService;
        }

        public async Task<Result<Guid>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();
            var productRepo = _unitOfWork.Repository<Product>();
            var categoryRepo = _unitOfWork.Repository<Category>();

            var category = categoryRepo.Query()
                .FirstOrDefault(c =>
                    c.Id == command.Request.CategoryId &&
                    c.BusinessId == command.BusinessId &&
                    !c.IsDeleted
                );

            if (category is null)
            {
                _logger.LogWarning("Categoría no válida. Id: {CategoryId}, BusinessId: {BusinessId}", command.Request.CategoryId, command.BusinessId);
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Fail(AppErrorFactory.NotFound(nameof(command.Request.CategoryId), command.Request.CategoryId));
            }

            var existing = productRepo.QueryIncludingDeleted()
                .FirstOrDefault(p =>
                    p.BusinessId == command.BusinessId &&
                    p.Name == command.Request.Name
                );

            if (existing is not null)
            {
                if (existing.IsDeleted)
                {
                    existing.IsDeleted = false;
                    existing.Price = command.Request.Price;
                    existing.CategoryId = command.Request.CategoryId;

                    productRepo.Update(existing);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation("Producto restaurado: {ProductId}", existing.Id);
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result.Ok(existing.Id);
                }

                _logger.LogWarning("Producto duplicado: {ProductName}", existing.Name);
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Fail(AppErrorFactory.Conflict("Ya existe un producto con ese nombre."));
            }

            var product = _mapper.Map<Product>(command.Request);
            product.BusinessId = command.BusinessId;

            var imageResult = await _productImageStorageService.UploadProductImagesAsync(product.Id, command.Request.Images, command.BusinessId, product);

            if (imageResult.IsFailed)
            {
                await _unitOfWork.RollbackTransactionAsync();
                var combinedMessages = string.Join(Environment.NewLine, imageResult.Errors.Select(e => e.Message));
                return Result.Fail(AppErrorFactory.Internal(combinedMessages ?? "No se pudo subir la imagen"));
            }

            await productRepo.AddAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Producto creado correctamente: {ProductId}", product.Id);
            return Result.Ok(product.Id);
        }
    }
}
