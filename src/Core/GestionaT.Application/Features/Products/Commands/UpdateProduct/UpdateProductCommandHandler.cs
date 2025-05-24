using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateProductCommandHandler> _logger;
        private readonly IMapper _mapper;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateProductCommandHandler> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var productRepo = _unitOfWork.Repository<Product>();
            var categoryRepo = _unitOfWork.Repository<Category>();

            var product = productRepo.Query()
                .FirstOrDefault(p => p.Id == command.ProductId && p.BusinessId == command.BusinessId);

            if (product is null)
            {
                _logger.LogWarning("Producto no encontrado: {ProductId}", command.ProductId);
                return Result.Fail(new HttpError("Producto no encontrado.", ResultStatusCode.NotFound));
            }

            var category = categoryRepo.Query()
                .FirstOrDefault(c =>
                    c.Id == command.Request.CategoryId &&
                    c.BusinessId == command.BusinessId &&
                    !c.IsDeleted);

            if (category is null)
            {
                _logger.LogWarning("Categoría no válida: {CategoryId}", command.Request.CategoryId);
                return Result.Fail(new HttpError("La categoría especificada no es válida o está eliminada.", ResultStatusCode.NotFound));
            }

            var exists = productRepo.Query()
                .Any(p =>
                    p.BusinessId == command.BusinessId &&
                    p.Name == command.Request.Name &&
                    p.Id != command.ProductId &&
                    !p.IsDeleted);

            if (exists)
            {
                _logger.LogWarning("Ya existe otro producto con el nombre: {Name}", command.Request.Name);
                return Result.Fail(new HttpError("Ya existe otro producto con ese nombre.", ResultStatusCode.Conflict));
            }

            _mapper.Map(command.Request, product);
            product.CategoryId = command.Request.CategoryId;

            productRepo.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Producto actualizado correctamente: {ProductId}", product.Id);
            return Result.Ok();
        }
    }
}
