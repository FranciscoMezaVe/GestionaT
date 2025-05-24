using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
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

        public CreateProductCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateProductCommandHandler> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
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
                return Result.Fail(new HttpError("La categoría especificada no es válida o está eliminada.", ResultStatusCode.NotFound));
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
                    return Result.Ok(existing.Id);
                }

                _logger.LogWarning("Producto duplicado: {ProductName}", existing.Name);
                return Result.Fail(new HttpError("Ya existe un producto con ese nombre.", ResultStatusCode.Conflict));
            }

            var product = _mapper.Map<Product>(command.Request);
            product.BusinessId = command.BusinessId;

            await productRepo.AddAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Producto creado correctamente: {ProductId}", product.Id);
            return Result.Ok(product.Id);
        }
    }
}
