using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace GestionaT.Application.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteProductCommandHandler> _logger;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteProductCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            var repository = _unitOfWork.Repository<Product>();

            var product = repository.QueryIncludingDeleted()
                .FirstOrDefault(p => p.Id == command.Id && p.BusinessId == command.BusinessId);

            if (product == null)
            {
                _logger.LogWarning("Producto no encontrado con ID {ProductId}", command.Id);
                return Result.Fail(new HttpError("Producto no encontrado.", ResultStatusCode.NotFound));
            }

            if (product.IsDeleted)
            {
                _logger.LogWarning("Producto ya está eliminado con ID {ProductId}", command.Id);
                return Result.Fail(new HttpError("El producto ya está eliminado.", ResultStatusCode.BadRequest));
            }

            // Soft delete
            repository.Remove(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Producto eliminado correctamente: {ProductId}", product.Id);
            return Result.Ok();
        }
    }
}
