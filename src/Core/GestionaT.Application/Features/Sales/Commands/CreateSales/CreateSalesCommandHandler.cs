using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Application.Profiles;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Sales.Commands.CreateSales
{
    public class CreateSalesCommandHandler : IRequestHandler<CreateSalesCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateSalesCommandHandler> _logger;

        public CreateSalesCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateSalesCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(CreateSalesCommand command, CancellationToken cancellationToken)
        {
            var customerIsExists = await _unitOfWork.Repository<Customer>().AnyAsync(c => c.Id == command.Request.CustomerId, cancellationToken);

            if(!customerIsExists)
            {
                _logger.LogWarning("Cliente con ID {command.Request.CustomerId} no existe.", command.Request.CustomerId);
                return Result.Fail(new HttpError($"Cliente con ID {command.Request.CustomerId} no existe.", ResultStatusCode.NotFound));
            }

            var productsIds = command.Request.Products.Select(p => p.Id).ToList();

            var productsIsExists = await _unitOfWork.Repository<Product>().AnyAsync(p => productsIds.Any(id => id == p.Id), cancellationToken);

            if (!productsIsExists)
            {
                _logger.LogWarning("Productos con IDs {command.Request.ProductsIds} no existen.", string.Join(", ", productsIds));
                return Result.Fail(new HttpError($"Productos con IDs {string.Join(", ", productsIds)} no existen.", ResultStatusCode.NotFound));
            }

            var sale = command.MapToSaleEntity();

            await _unitOfWork.Repository<Sale>().AddAsync(sale);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (result <= 0)
                return Result.Fail("No se pudo guardar la venta");

            return Result.Ok(sale.Id);
        }
    }
}
