using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using GestionaT.Shared.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCustomerCommandHandler> _logger;

        public UpdateCustomerCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UpdateCustomerCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateCustomerCommand command, CancellationToken cancellationToken)
        {
            var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(command.Id);

            if (customer == null)
            {
                _logger.LogWarning("No se encontró el cliente con ID {CustomerId}", command.Id);
                return Result.Fail(new HttpError("Cliente no encontrado.", ResultStatusCode.NotFound));
            }

            // Validar que el negocio exista
            var business = await _unitOfWork.Repository<Domain.Entities.Business>().GetByIdAsync(command.BusinessId);
            if (business == null)
            {
                _logger.LogWarning("Negocio no encontrado al actualizar cliente: {BusinessId}", command.BusinessId);
                return Result.Fail(new HttpError("El negocio no existe.", ResultStatusCode.NotFound));
            }

            // Mapear valores nuevos al entity existente
            _mapper.Map(command.Request, customer);
            _unitOfWork.Repository<Customer>().Update(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Cliente {CustomerId} actualizado exitosamente", command.Id);
            return Result.Ok();
        }
    }
}