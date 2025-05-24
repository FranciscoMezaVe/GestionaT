using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Customers.Queries.GetAllCustomers
{
    public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, Result<List<CustomerResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllCustomersQueryHandler> _logger;

        public GetAllCustomersQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetAllCustomersQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<List<CustomerResponse>>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            var customers = _unitOfWork.Repository<Customer>()
                .Query()
                .Where(c => c.BusinessId == request.BusinessId)
                .ProjectTo<CustomerResponse>(_mapper.ConfigurationProvider)
                .ToList();

            if(customers.Count == 0)
            {
                _logger.LogWarning("No se encontraron clientes para el negocio {BusinessId}", request.BusinessId);
                return Result.Fail(new HttpError("No se encontraron clientes.", ResultStatusCode.NotFound));
            }

            _logger.LogInformation("Se obtuvieron {Count} clientes del negocio {BusinessId}", customers.Count, request.BusinessId);
            return Result.Ok(customers);
        }
    }
}
