using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Customers.Commands.CreateCustomer
{
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateCustomerCommandHandler> _logger;

        public CreateCustomerCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateCustomerCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
        {
            // Validar que el negocio exista y no esté eliminado
            var business = await _unitOfWork.Repository<Domain.Entities.Business>().GetByIdAsync(command.BusinessId);
            if (business == null)
            {
                _logger.LogWarning("Intento de crear cliente con negocio inexistente: {BusinessId}", command.BusinessId);
                return Result.Fail(new HttpError("El negocio no existe.", ResultStatusCode.NotFound));
            }

            var customer = _mapper.Map<Customer>(command.Request);
            customer.BusinessId = command.BusinessId;
            await _unitOfWork.Repository<Customer>().AddAsync(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Cliente creado exitosamente con ID {CustomerId}", customer.Id);

            return Result.Ok(customer.Id);
        }
    }
}