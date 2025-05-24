using FluentResults;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Customers.Commands.DeleteCustomer
{
    public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteCustomerCommandHandler> _logger;

        public DeleteCustomerCommandHandler(ILogger<DeleteCustomerCommandHandler> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(request.CustomerId);
            if (customer == null || customer.BusinessId != request.BusinessId)
            {
                _logger.LogWarning("Intento de eliminar cliente no encontrado o que no pertenece al negocio. CustomerId: {CustomerId}, BusinessId: {BusinessId}", request.CustomerId, request.BusinessId);
                return Result.Fail("Cliente no encontrado o no pertenece al negocio.");
            }

            _unitOfWork.Repository<Customer>().Remove(customer);

            try
            {
                // Asumiendo que tu UnitOfWork o contexto se guardará después
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Cliente eliminado correctamente. CustomerId: {CustomerId}", request.CustomerId);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar cliente. CustomerId: {CustomerId}", request.CustomerId);
                return Result.Fail("Error al eliminar el cliente.");
            }
        }
    }
}